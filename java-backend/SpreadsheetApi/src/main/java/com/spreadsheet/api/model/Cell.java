package com.spreadsheet.api.model;

import java.time.LocalDateTime;


public class Cell {
    private int row;
    private int col;
    private String value;
    private LocalDateTime lastModified;

    public Cell(int row, int col, String value, LocalDateTime lastModified) {
        this.row = row;
        this.col = col;
        this.value = value;
        this.lastModified = lastModified;
    }

    public int getRow() { return row; }
    public int getCol() { return col; }
    public String getValue() { return value; }
    public LocalDateTime getLastModified() { return lastModified; }

    public void setValue(String value) { this.value = value; }
    public void setLastModified(LocalDateTime lastModified) { this.lastModified = lastModified; }
}