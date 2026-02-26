package com.spreadsheet.api.repository;

import com.spreadsheet.api.db.DatabaseConnection;
import com.spreadsheet.api.model.Cell;
import com.spreadsheet.api.model.GridData;

import java.sql.*;
import java.util.ArrayList;
import java.util.List;


public class GridRepository implements IGridRepository {

    @Override
    public List<GridData> getAllGrids() throws SQLException {
        List<GridData> grids = new ArrayList<>();
        String sql = "SELECT GridId, Name, TotalRows, TotalColumns " +
                     "FROM Grids ORDER BY DateUpdated DESC";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql);
             ResultSet rs = stmt.executeQuery()) {

            while (rs.next()) {
                grids.add(new GridData(
                    rs.getInt("GridId"),
                    rs.getString("Name"),
                    rs.getInt("TotalRows"),
                    rs.getInt("TotalColumns"),
                    new ArrayList<>()
                ));
            }
        }
        return grids;
    }

    @Override
    public GridData getGridById(int gridId) throws SQLException {
        String sql = "SELECT GridId, Name, TotalRows, TotalColumns " +
                     "FROM Grids WHERE GridId = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setInt(1, gridId);
            try (ResultSet rs = stmt.executeQuery()) {
                if (rs.next()) {
                    return new GridData(
                        rs.getInt("GridId"),
                        rs.getString("Name"),
                        rs.getInt("TotalRows"),
                        rs.getInt("TotalColumns"),
                        new ArrayList<>()
                    );
                }
            }
        }
        return null;
    }

    @Override
    public int createGrid(String name) throws SQLException {
        String sql = "INSERT INTO Grids (Name, TotalRows, TotalColumns, DateCreated, DateUpdated) " +
                     "VALUES (?, 20, 20, GETDATE(), GETDATE())";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql, Statement.RETURN_GENERATED_KEYS)) {

            stmt.setString(1, name);
            stmt.executeUpdate();

            try (ResultSet keys = stmt.getGeneratedKeys()) {
                keys.next();
                return keys.getInt(1);
            }
        }
    }

    @Override
    public void updateDimensions(int gridId, int totalRows, int totalColumns) throws SQLException {
        String sql = "UPDATE Grids SET TotalRows = ?, TotalColumns = ?, " +
                     "DateUpdated = GETDATE() WHERE GridId = ?";

        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {

            stmt.setInt(1, totalRows);
            stmt.setInt(2, totalColumns);
            stmt.setInt(3, gridId);
            stmt.executeUpdate();
        }
    }

    @Override
    public void incrementTotalRows(int gridId) throws SQLException {
        executeDimensionUpdate(
            "UPDATE Grids SET TotalRows = TotalRows + 1, DateUpdated = GETDATE() WHERE GridId = ?",
            gridId
        );
    }

    @Override
    public void decrementTotalRows(int gridId) throws SQLException {
        executeDimensionUpdate(
            "UPDATE Grids SET TotalRows = TotalRows - 1, DateUpdated = GETDATE() WHERE GridId = ?",
            gridId
        );
    }

    @Override
    public void incrementTotalColumns(int gridId) throws SQLException {
        executeDimensionUpdate(
            "UPDATE Grids SET TotalColumns = TotalColumns + 1, DateUpdated = GETDATE() WHERE GridId = ?",
            gridId
        );
    }

    @Override
    public void decrementTotalColumns(int gridId) throws SQLException {
        executeDimensionUpdate(
            "UPDATE Grids SET TotalColumns = TotalColumns - 1, DateUpdated = GETDATE() WHERE GridId = ?",
            gridId
        );
    }

    @Override
    public void updateTimestamp(int gridId) throws SQLException {
        executeDimensionUpdate(
            "UPDATE Grids SET DateUpdated = GETDATE() WHERE GridId = ?",
            gridId
        );
    }

    private void executeDimensionUpdate(String sql, int gridId) throws SQLException {
        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement stmt = conn.prepareStatement(sql)) {
            stmt.setInt(1, gridId);
            stmt.executeUpdate();
        }
    }
}