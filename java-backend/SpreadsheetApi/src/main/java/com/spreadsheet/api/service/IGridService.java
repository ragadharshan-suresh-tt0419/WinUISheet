package com.spreadsheet.api.service;

import com.spreadsheet.api.model.Cell;
import com.spreadsheet.api.model.GridData;

import java.sql.SQLException;
import java.util.List;


public interface IGridService {
    List<GridData> getAllGrids() throws SQLException;
    GridData getGridData(int gridId) throws SQLException;
    int createGrid(String name) throws SQLException;
    void updateCell(int gridId, int row, int col, String value) throws SQLException;
    void insertRow(int gridId, int index) throws SQLException;
    void deleteRow(int gridId, int index) throws SQLException;
    void insertColumn(int gridId, int index) throws SQLException;
    void deleteColumn(int gridId, int index) throws SQLException;
    void saveAllCells(int gridId, int totalRows, int totalColumns, List<Cell> cells) throws SQLException;
}