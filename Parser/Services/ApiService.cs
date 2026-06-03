using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Parser.Models;

namespace Parser.Services
{
    public interface IApiService
    {
        Task<List<Branch>> GetBranchesAsync();
        Task<List<Year>> GetYearsAsync();
        Task<List<Group>> GetGroupsAsync(string branchId, string yearId);
        Task<ScheduleResponse> GetScheduleForGroupAsync(string branchGuid, string groupId, string mondayDate);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api-schedule.ruc.su";
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        private bool _sessionInitialized = false;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36 Edg/147.0.0.0");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://new-schedule.ruc.su");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://new-schedule.ruc.su/");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        private async Task InitializeSessionAsync()
        {
            if (_sessionInitialized) return;
            var homeUrl = "https://new-schedule.ruc.su/";
            using var request = new HttpRequestMessage(HttpMethod.Get, homeUrl);
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36 Edg/147.0.0.0");
            request.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            request.Headers.Add("Accept-Language", "ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            request.Headers.Add("Cache-Control", "no-cache");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            _sessionInitialized = true;
        }

        private async Task<string> SendRequestAsync(string url)
        {
            await InitializeSessionAsync();
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Accept", "application/json, text/plain, */*");
            request.Headers.Add("Accept-Language", "ru,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");
            request.Headers.Add("Origin", "https://new-schedule.ruc.su");
            request.Headers.Add("Referer", "https://new-schedule.ruc.su/");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.Headers.Add("Priority", "u=1, i");
            request.Headers.Add("Sec-Ch-Ua", "\"Microsoft Edge\";v=\"147\", \"NotA/Brand\";v=\"8\", \"Chromium\";v=\"147\"");
            request.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
            request.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36 Edg/147.0.0.0");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"API ответ от {url}: {json}");
            return json;
        }

        public async Task<List<Branch>> GetBranchesAsync()
        {
            var url = $"{_baseUrl}/api/v1/get_branches";
            var json = await SendRequestAsync(url);
            return JsonSerializer.Deserialize<List<Branch>>(json, _jsonOptions);
        }

        public async Task<List<Year>> GetYearsAsync()
        {
            var url = $"{_baseUrl}/api/v1/get_years";
            var json = await SendRequestAsync(url);
            return JsonSerializer.Deserialize<List<Year>>(json, _jsonOptions);
        }

        public async Task<List<Group>> GetGroupsAsync(string branchId, string yearId)
        {
            var url = $"{_baseUrl}/api/v1/get_groups_filter/{branchId}/{yearId}";
            var json = await SendRequestAsync(url);
            var categories = JsonSerializer.Deserialize<List<GroupCategory>>(json, _jsonOptions);
            var allGroups = new List<Group>();
            if (categories != null)
            {
                foreach (var cat in categories)
                {
                    if (cat.Groups != null)
                        allGroups.AddRange(cat.Groups);
                }
            }
            Debug.WriteLine($"Извлечено групп: {allGroups.Count}");
            return allGroups;
        }

        public async Task<ScheduleResponse> GetScheduleForGroupAsync(string branchGuid, string groupId, string mondayDate)
        {
            var url = $"{_baseUrl}/api/v1/get_schedule/group/{branchGuid}/{groupId}/{mondayDate}";
            var json = await SendRequestAsync(url);
            return JsonSerializer.Deserialize<ScheduleResponse>(json, _jsonOptions);
        }
    }
}