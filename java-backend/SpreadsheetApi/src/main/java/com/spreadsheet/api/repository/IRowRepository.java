package com.spreadsheet.api.repository;

import java.sql.Connection;
import java.sql.SQLException;


public interface IRowRepository {
    int getOrCreateRow(Connection conn, int gridId, int rowIndex) throws SQLException;
    void insertRow(int gridId, int rowIndex) throws SQLException;
    void deleteRow(int gridId, int rowIndex) throws SQLException;
    void shiftRowsDown(int gridId, int fromIndex) throws SQLException;
    void shiftRowsUp(int gridId, int fromIndex) throws SQLException;
    void deleteAllRows(int gridId, Connection conn) throws SQLException;
}