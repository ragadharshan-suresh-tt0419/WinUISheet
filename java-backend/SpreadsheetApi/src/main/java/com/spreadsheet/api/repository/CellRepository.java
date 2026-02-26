package com.spreadsheet.api.repository;

import com.spreadsheet.api.db.DatabaseConnection;
import com.spreadsheet.api.model.Cell;

import java.sql.*;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;


public class CellRepository implements ICellRepository {

    @Override
    public List<Cell> getCellsByGridId(int gridId) throws SQLException {
        List<Cell> cells = new ArrayList<>();
        String sql = "SELECT r.RowIndex, c.ColumnIndex, c.Value, c.LastModified " +
                     "FROM Cells c " +
                     "JOIN Rows r ON c.RowId = r.RowId " +
                     "WHERE r.GridId = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setInt(1, gridId);
            try (ResultSet rs = stmt.executeQuery()) {
                while (rs.next()) {
                    Timestamp ts = rs.getTimestamp("LastModified");
                    LocalDateTime lastModified = ts != null
                        ? ts.toLocalDateTime()
                        : LocalDateTime.now();

                    cells.add(new Cell(
                        rs.getInt("RowIndex"),
                        rs.getInt("ColumnIndex"),
                        rs.getString("Value"),
                        lastModified
                    ));
                }
            }
        }
        return cells;
    }

    @Override
    public void upsertCell(Connection conn, int rowId, int col, String value) throws SQLException {
        String sql = "MERGE INTO Cells AS target " +
                     "USING (VALUES (?, ?, ?, GETDATE())) " +
                     "  AS source (RowId, ColumnIndex, Value, LastModified) " +
                     "ON target.RowId = source.RowId " +
                     "   AND target.ColumnIndex = source.ColumnIndex " +
                     "WHEN MATCHED THEN " +
                     "  UPDATE SET Value = source.Value, LastModified = source.LastModified " +
                     "WHEN NOT MATCHED THEN " +
                     "  INSERT (RowId, ColumnIndex, Value, LastModified) " +
                     "  VALUES (source.RowId, source.ColumnIndex, source.Value, source.LastModified);";

        try (PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, rowId);
            stmt.setInt(2, col);
            stmt.setString(3, value);
            stmt.executeUpdate();
        }
    }

    @Override
    public void deleteCell(int rowId, int col) throws SQLException {
        String sql = "DELETE FROM Cells WHERE RowId = ? AND ColumnIndex = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, rowId);
            stmt.setInt(2, col);
            stmt.executeUpdate();
        }
    }

    @Override
    public void deleteAllCellsInColumn(int gridId, int colIndex) throws SQLException {
        String sql = "DELETE FROM Cells " +
                     "WHERE RowId IN (SELECT RowId FROM Rows WHERE GridId = ?) " +
                     "AND ColumnIndex = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, colIndex);
            stmt.executeUpdate();
        }
    }

    @Override
    public void shiftColumnsRight(int gridId, int fromIndex) throws SQLException {
        String sql = "UPDATE Cells SET ColumnIndex = ColumnIndex + 1 " +
                     "WHERE RowId IN (SELECT RowId FROM Rows WHERE GridId = ?) " +
                     "AND ColumnIndex >= ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, fromIndex);
            stmt.executeUpdate();
        }
    }

    @Override
    public void shiftColumnsLeft(int gridId, int fromIndex) throws SQLException {
        String sql = "UPDATE Cells SET ColumnIndex = ColumnIndex - 1 " +
                     "WHERE RowId IN (SELECT RowId FROM Rows WHERE GridId = ?) " +
                     "AND ColumnIndex > ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, fromIndex);
            stmt.executeUpdate();
        }
    }
}