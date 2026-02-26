package com.spreadsheet.api.repository;

import com.spreadsheet.api.model.Cell;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;


public interface ICellRepository {
    List<Cell> getCellsByGridId(int gridId) throws SQLException;
    void upsertCell(Connection conn, int rowId, int col, String value) throws SQLException;
    void deleteCell(int rowId, int col) throws SQLException;
    void deleteAllCellsInColumn(int gridId, int colIndex) throws SQLException;
    void shiftColumnsRight(int gridId, int fromIndex) throws SQLException;
    void shiftColumnsLeft(int gridId, int fromIndex) throws SQLException;
}