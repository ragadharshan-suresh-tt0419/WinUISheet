package com.spreadsheet.api.repository;

import com.spreadsheet.api.db.DatabaseConnection;

import java.sql.*;


public class RowRepository implements IRowRepository {

    @Override
    public int getOrCreateRow(Connection conn, int gridId, int rowIndex) throws SQLException {
        String selectSql = "SELECT RowId FROM Rows WHERE GridId = ? AND RowIndex = ?";

        try (PreparedStatement stmt = conn.prepareStatement(selectSql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, rowIndex);
            try (ResultSet rs = stmt.executeQuery()) {
                if (rs.next()) {
                    return rs.getInt("RowId");
                }
            }
        }

        String insertSql = "INSERT INTO Rows (GridId, RowIndex) VALUES (?, ?)";
        try (PreparedStatement stmt = conn.prepareStatement(insertSql, Statement.RETURN_GENERATED_KEYS)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, rowIndex);
            stmt.executeUpdate();
            try (ResultSet keys = stmt.getGeneratedKeys()) {
                keys.next();
                return keys.getInt(1);
            }
        }
    }

    @Override
    public void insertRow(int gridId, int rowIndex) throws SQLException {
        String sql = "INSERT INTO Rows (GridId, RowIndex) VALUES (?, ?)";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, rowIndex);
            stmt.executeUpdate();
        }
    }

    @Override
    public void deleteRow(int gridId, int rowIndex) throws SQLException {
        String sql = "DELETE FROM Rows WHERE GridId = ? AND RowIndex = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, rowIndex);
            stmt.executeUpdate();
        }
    }

    @Override
    public void shiftRowsDown(int gridId, int fromIndex) throws SQLException {
        String sql = "UPDATE Rows SET RowIndex = RowIndex + 1 " +
                     "WHERE GridId = ? AND RowIndex >= ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, fromIndex);
            stmt.executeUpdate();
        }
    }

    @Override
    public void shiftRowsUp(int gridId, int fromIndex) throws SQLException {
        String sql = "UPDATE Rows SET RowIndex = RowIndex - 1 " +
                     "WHERE GridId = ? AND RowIndex > ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.setInt(2, fromIndex);
            stmt.executeUpdate();
        }
    }


    @Override
    public void deleteAllRows(int gridId, Connection conn) throws SQLException {
        String sql = "DELETE FROM Rows WHERE GridId = ?";

        try (PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.executeUpdate();
        }
    }
}