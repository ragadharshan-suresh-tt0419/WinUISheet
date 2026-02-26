package com.spreadsheet.api.model;

import java.util.List;


public class GridData {
    private int gridId;
    private String name;
    private int totalRows;
    private int totalColumns;
    private List<Cell> cells;

    public GridData(int gridId, String name, int totalRows, int totalColumns, List<Cell> cells) {
        this.gridId = gridId;
        this.name = name;
        this.totalRows = totalRows;
        this.totalColumns = totalColumns;
        this.cells = cells;
    }

    public int getGridId() { return gridId; }
    public String getName() { return name; }
    public int getTotalRows() { return totalRows; }
    public int getTotalColumns() { return totalColumns; }
    public List<Cell> getCells() { return cells; }

    public void setTotalRows(int totalRows) { this.totalRows = totalRows; }
    public void setTotalColumns(int totalColumns) { this.totalColumns = totalColumns; }
}