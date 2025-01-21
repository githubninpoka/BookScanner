using EbooksIndex.ClassLibrary.DataAccess;
using EbooksIndex.ClassLibrary.Interfaces;
using OpenSearch.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbooksIndex.ClassLibrary;

public class RepositoryHelper : IRepositoryHelper
{

    private OpenSearchClient _db;

    public RepositoryHelper(OpenSearchAccess openSearchAccess)
    {
        _db = openSearchAccess.GetCLient();
    }
    public async Task<int> GetStatusAsync()
    {
        var health = await _db.Cluster.HealthAsync();
        if (health.IsValid && health.Status == OpenSearch.Net.Health.Green)
        {
            return 1;
        }else if (health.IsValid && health.Status == OpenSearch.Net.Health.Yellow)
        {
            return 2;
        }
        return 3;
    }
}
