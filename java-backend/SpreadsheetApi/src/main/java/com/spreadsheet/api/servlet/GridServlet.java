package com.spreadsheet.api.servlet;

import com.google.gson.*;
import com.spreadsheet.api.model.Cell;
import com.spreadsheet.api.model.GridData;
import com.spreadsheet.api.repository.CellRepository;
import com.spreadsheet.api.repository.GridRepository;
import com.spreadsheet.api.repository.RowRepository;
import com.spreadsheet.api.service.GridService;
import com.spreadsheet.api.service.IGridService;

import jakarta.servlet.ServletException;
import jakarta.servlet.annotation.WebServlet;
import jakarta.servlet.http.HttpServlet;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import java.io.BufferedReader;
import java.io.IOException;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

/**

 */
@WebServlet("/api/grid/*")
public class GridServlet extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private IGridService gridService;
    private final Gson gson = new Gson();

    @Override
    public void init() throws ServletException {

        gridService = new GridService(
            new GridRepository(),
            new CellRepository(),
            new RowRepository()
        );
    }

    @Override
    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        setCorsHeaders(response);
        response.setContentType("application/json");
        response.setCharacterEncoding("UTF-8");

        String pathInfo = request.getPathInfo();

        try {
            if (pathInfo == null || pathInfo.equals("/")) {
                handleGetAllGrids(response);
            } else {
                int gridId = Integer.parseInt(pathInfo.substring(1));
                handleGetGridData(gridId, response);
            }
        } catch (NumberFormatException e) {
            sendError(response, 400, "Invalid grid ID");
        } catch (SQLException e) {
            sendError(response, 500, e.getMessage());
        }
    }

    private void handleGetAllGrids(HttpServletResponse response)
            throws SQLException, IOException {

        List<GridData> grids = gridService.getAllGrids();
        JsonArray result = new JsonArray();

        for (GridData g : grids) {
            JsonObject obj = new JsonObject();
            obj.addProperty("gridId", g.getGridId());
            obj.addProperty("name", g.getName());
            obj.addProperty("totalRows", g.getTotalRows());
            obj.addProperty("totalColumns", g.getTotalColumns());
            result.add(obj);
        }

        response.getWriter().write(gson.toJson(result));
    }

    private void handleGetGridData(int gridId, HttpServletResponse response)
            throws SQLException, IOException {

        GridData grid = gridService.getGridData(gridId);

        if (grid == null) {
            sendError(response, 404, "Grid not found");
            return;
        }

        JsonObject result = new JsonObject();
        result.addProperty("gridId", grid.getGridId());
        result.addProperty("totalRows", grid.getTotalRows());
        result.addProperty("totalColumns", grid.getTotalColumns());

        JsonArray cells = new JsonArray();
        for (Cell cell : grid.getCells()) {
            JsonObject c = new JsonObject();
            c.addProperty("row", cell.getRow());
            c.addProperty("col", cell.getCol());
            c.addProperty("value", cell.getValue());
            c.addProperty("lastModified", cell.getLastModified().toString());
            cells.add(c);
        }
        result.add("cells", cells);

        response.getWriter().write(gson.toJson(result));
    }

    @Override
    protected void doPost(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {

        setCorsHeaders(response);
        response.setContentType("application/json");

        JsonObject body = parseBody(request);
        String action = body.has("action") ? body.get("action").getAsString() : "";

        try {
            switch (action) {
                case "createGrid"   -> handleCreateGrid(body, response);
                case "updateCell"   -> handleUpdateCell(body, response);
                case "insertRow"    -> handleInsertRow(body, response);
                case "deleteRow"    -> handleDeleteRow(body, response);
                case "insertCol"    -> handleInsertCol(body, response);
                case "deleteCol"    -> handleDeleteCol(body, response);
                case "saveAllCells" -> handleSaveAllCells(body, response);
                default             -> sendError(response, 400, "Unknown action: " + action);
            }
        } catch (SQLException e) {
            sendError(response, 500, e.getMessage());
        }
    }

    private void handleCreateGrid(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        String name = body.get("name").getAsString();
        int gridId = gridService.createGrid(name);

        JsonObject result = new JsonObject();
        result.addProperty("success", true);
        result.addProperty("gridId", gridId);
        response.getWriter().write(gson.toJson(result));
    }

    private void handleUpdateCell(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        int gridId = body.get("gridId").getAsInt();
        int row    = body.get("row").getAsInt();
        int col    = body.get("col").getAsInt();
        String value = body.get("value").getAsString();

        gridService.updateCell(gridId, row, col, value);
        response.getWriter().write("{\"success\": true}");
    }

    private void handleInsertRow(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        gridService.insertRow(body.get("gridId").getAsInt(), body.get("index").getAsInt());
        response.getWriter().write("{\"success\": true}");
    }

    private void handleDeleteRow(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        gridService.deleteRow(body.get("gridId").getAsInt(), body.get("index").getAsInt());
        response.getWriter().write("{\"success\": true}");
    }

    private void handleInsertCol(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        gridService.insertColumn(body.get("gridId").getAsInt(), body.get("index").getAsInt());
        response.getWriter().write("{\"success\": true}");
    }

    private void handleDeleteCol(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        gridService.deleteColumn(body.get("gridId").getAsInt(), body.get("index").getAsInt());
        response.getWriter().write("{\"success\": true}");
    }

    private void handleSaveAllCells(JsonObject body, HttpServletResponse response)
            throws SQLException, IOException {

        int gridId       = body.get("gridId").getAsInt();
        int totalRows    = body.get("totalRows").getAsInt();
        int totalColumns = body.get("totalColumns").getAsInt();
        JsonArray rawCells = body.getAsJsonArray("cells");

        List<Cell> cells = new ArrayList<>();
        for (JsonElement el : rawCells) {
            JsonObject c = el.getAsJsonObject();
            cells.add(new Cell(
                c.get("row").getAsInt(),
                c.get("col").getAsInt(),
                c.get("value").getAsString(),
                java.time.LocalDateTime.now()
            ));
        }

        gridService.saveAllCells(gridId, totalRows, totalColumns, cells);
        response.getWriter().write("{\"success\": true}");
    }

    @Override
    protected void doOptions(HttpServletRequest request, HttpServletResponse response) {
        setCorsHeaders(response);
        response.setHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        response.setHeader("Access-Control-Allow-Headers", "Content-Type");
        response.setStatus(200);
    }

    private void setCorsHeaders(HttpServletResponse response) {
        response.setHeader("Access-Control-Allow-Origin", "*");
    }

    private JsonObject parseBody(HttpServletRequest request) throws IOException {
        BufferedReader reader = request.getReader();
        StringBuilder sb = new StringBuilder();
        String line;
        while ((line = reader.readLine()) != null) sb.append(line);
        return gson.fromJson(sb.toString(), JsonObject.class);
    }

    private void sendError(HttpServletResponse response, int status, String message)
            throws IOException {
        response.setStatus(status);
        response.getWriter().write("{\"error\": \"" + message + "\"}");
    }
}