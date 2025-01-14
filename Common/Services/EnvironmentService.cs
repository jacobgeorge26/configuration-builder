﻿using System.Collections;
using Common.Services.Interfaces;

namespace Common.Services;

public class EnvironmentService : IEnvironmentService
{
    public Dictionary<string, string?> GetEnvironmentVariables()
    {
        var vars = Environment.GetEnvironmentVariables();
        var dict = new Dictionary<string, string?>();
        foreach (DictionaryEntry entry in vars)
        {
            var key = entry.Key.ToString();
            if(key is null)
                continue;
            
            dict.Add(key, entry.Value?.ToString());
        }
        
        return dict;
    }
}