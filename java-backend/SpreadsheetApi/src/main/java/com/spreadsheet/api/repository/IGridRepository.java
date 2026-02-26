package com.spreadsheet.api.repository;

import com.spreadsheet.api.model.GridData;
import java.sql.SQLException;
import java.util.List;


public interface IGridRepository {
    List<GridData> getAllGrids() throws SQLException;
    GridData getGridById(int gridId) throws SQLException;
    int createGrid(String name) throws SQLException;
    void updateDimensions(int gridId, int totalRows, int totalColumns) throws SQLException;
    void incrementTotalRows(int gridId) throws SQLException;
    void decrementTotalRows(int gridId) throws SQLException;
    void incrementTotalColumns(int gridId) throws SQLException;
    void decrementTotalColumns(int gridId) throws SQLException;
    void updateTimestamp(int gridId) throws SQLException;
}