﻿namespace BookMarkBrain.MVC.Services.APIServiceInterfaces;

public interface IApiService
{
    Task<T> GetAsync<T>(string endpoint);
    Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<bool> DeleteAsync(string endpoint);
}
