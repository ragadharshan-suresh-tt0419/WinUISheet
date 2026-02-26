using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace gridLevel2LL.Services
{
    public class CellDto
    {
        public int row { get; set; }
        public int col { get; set; }
        public string value { get; set; }
    }

    public class GridDataDto
    {
        public int gridId { get; set; }
        public int totalRows { get; set; }
        public int totalColumns { get; set; }
        public List<CellDto> cells { get; set; }
    }

    public class GridDto
    {
        public int gridId { get; set; }
        public string name { get; set; }
        public string dateCreated { get; set; }
        public string dateUpdated { get; set; }
    }

    public class GridApiService
    {
        private readonly HttpClient httpClient;
        private const string BASE_URL = "http://localhost:8081/SpreadsheetApi/api/grid";

        public GridApiService()
        {
            httpClient = new HttpClient();
        }

        public async Task<List<GridDto>> GetAllGridsAsync()
        {
            var response = await httpClient.GetAsync(BASE_URL);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<GridDto>>();
        }

        public async Task<GridDataDto> GetGridDataAsync(int gridId)
        {
            var response = await httpClient.GetAsync($"{BASE_URL}/{gridId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<GridDataDto>();
        }

        public async Task<bool> UpdateCellAsync(int gridId, int row, int col, string value)
        {
            var payload = new { action = "updateCell", gridId, row, col, value };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> SaveAllCellsAsync(int gridId, int totalRows, int totalColumns,
            List<(int row, int col, string value)> cells)
        {
            var payload = new
            {
                action = "saveAllCells",
                gridId,
                totalRows,
                totalColumns,
                cells = cells.ConvertAll(c => new { row = c.row, col = c.col, value = c.value })
            };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> CreateGridAsync(string name)
        {
            var payload = new { action = "createGrid", name };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);

            var raw = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"API Error: {raw}");
            }

            var result = System.Text.Json.JsonSerializer
                .Deserialize<Dictionary<string, object>>(raw);

            return Convert.ToInt32(result["gridId"]);
        }

        public async Task<bool> InsertRowAsync(int gridId, int index)
        {
            var payload = new { action = "insertRow", gridId, index };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteRowAsync(int gridId, int index)
        {
            var payload = new { action = "deleteRow", gridId, index };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> InsertColAsync(int gridId, int index)
        {
            var payload = new { action = "insertCol", gridId, index };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteColAsync(int gridId, int index)
        {
            var payload = new { action = "deleteCol", gridId, index };
            var response = await httpClient.PostAsJsonAsync(BASE_URL, payload);
            return response.IsSuccessStatusCode;
        }
    }
}