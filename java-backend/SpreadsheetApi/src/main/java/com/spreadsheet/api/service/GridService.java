package com.spreadsheet.api.service;

import com.spreadsheet.api.db.DatabaseConnection;
import com.spreadsheet.api.model.Cell;
import com.spreadsheet.api.model.GridData;
import com.spreadsheet.api.repository.*;

import java.sql.Connection;
import java.sql.SQLException;
import java.util.List;


public class GridService implements IGridService {

    private final IGridRepository gridRepo;
    private final ICellRepository cellRepo;
    private final IRowRepository rowRepo;

    public GridService(IGridRepository gridRepo,
                       ICellRepository cellRepo,
                       IRowRepository rowRepo) {
        this.gridRepo = gridRepo;
        this.cellRepo = cellRepo;
        this.rowRepo = rowRepo;
    }

    @Override
    public List<GridData> getAllGrids() throws SQLException {
        return gridRepo.getAllGrids();
    }

    @Override
    public GridData getGridData(int gridId) throws SQLException {
        GridData grid = gridRepo.getGridById(gridId);
        if (grid == null) return null;

        List<Cell> cells = cellRepo.getCellsByGridId(gridId);
        grid.getCells().addAll(cells);

        int maxRow = Math.max(20, grid.getTotalRows());
        int maxCol = Math.max(20, grid.getTotalColumns());

        for (Cell cell : cells) {
            if (cell.getRow() + 1 > maxRow) maxRow = cell.getRow() + 1;
            if (cell.getCol() + 1 > maxCol) maxCol = cell.getCol() + 1;
        }

        grid.setTotalRows(maxRow);
        grid.setTotalColumns(maxCol);

        return grid;
    }

    @Override
    public int createGrid(String name) throws SQLException {
        return gridRepo.createGrid(name);
    }

    @Override
    public void updateCell(int gridId, int row, int col, String value) throws SQLException {
        try (Connection conn = DatabaseConnection.getConnection()) {
            int rowId = rowRepo.getOrCreateRow(conn, gridId, row);

            if (value == null || value.trim().isEmpty()) {
                cellRepo.deleteCell(rowId, col);
            } else {
                cellRepo.upsertCell(conn, rowId, col, value);
            }

            gridRepo.updateTimestamp(gridId);
        }
    }

    @Override
    public void insertRow(int gridId, int index) throws SQLException {
        rowRepo.shiftRowsDown(gridId, index);
        rowRepo.insertRow(gridId, index);
        gridRepo.incrementTotalRows(gridId);
    }

    @Override
    public void deleteRow(int gridId, int index) throws SQLException {
        rowRepo.deleteRow(gridId, index);
        rowRepo.shiftRowsUp(gridId, index);
        gridRepo.decrementTotalRows(gridId);
    }

    @Override
    public void insertColumn(int gridId, int index) throws SQLException {
        cellRepo.shiftColumnsRight(gridId, index);
        gridRepo.incrementTotalColumns(gridId);
    }

    @Override
    public void deleteColumn(int gridId, int index) throws SQLException {
        cellRepo.deleteAllCellsInColumn(gridId, index);
        cellRepo.shiftColumnsLeft(gridId, index);
        gridRepo.decrementTotalColumns(gridId);
    }

    @Override
    public void saveAllCells(int gridId, int totalRows, int totalColumns,
                             List<Cell> cells) throws SQLException {

        try (Connection conn = DatabaseConnection.getConnection()) {
            conn.setAutoCommit(false);
            try {

                rowRepo.deleteAllRows(gridId, conn);

                
                for (Cell cell : cells) {
                    if (cell.getValue() == null || cell.getValue().trim().isEmpty()) continue;
                    int rowId = rowRepo.getOrCreateRow(conn, gridId, cell.getRow());
                    cellRepo.upsertCell(conn, rowId, cell.getCol(), cell.getValue());
                }

                conn.commit();
            } catch (SQLException ex) {
                conn.rollback();
                throw ex;
            } finally {
                conn.setAutoCommit(true);
            }
        }

        gridRepo.updateDimensions(gridId, totalRows, totalColumns);
    }
}