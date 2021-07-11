using System;
using System.Collections.Generic;

namespace mini_ITS.Core.Database
{
    public class SqlPagedResult<T> : SqlPagedResultBase
    {
        public IEnumerable<T> Results { get; set; }

        public static SqlPagedResult<T> From(SqlPagedResultBase pagedResult, IEnumerable<T> results)
            => new SqlPagedResult<T>
            {
                Results = results,
                CurrentPage = pagedResult.CurrentPage,
                ResultsPerPage = pagedResult.ResultsPerPage,
                TotalResults = pagedResult.TotalResults,
                TotalPages = pagedResult.TotalPages
            };

        public static SqlPagedResult<T> Create(IEnumerable<T> results, 
            SqlPagedQueryBase query, int totalResults)
            => Create(results, query.Page, query.ResultsPerPage, totalResults);

        public static SqlPagedResult<T> Create(IEnumerable<T> results, 
            int currentPage, int resultsPerPage, int totalResults)
            {
                return new SqlPagedResult<T>
                {
                    Results = results,
                    CurrentPage = currentPage,
                    ResultsPerPage = resultsPerPage,
                    TotalResults = totalResults,
                    TotalPages = (int)Math.Ceiling((double)totalResults/resultsPerPage)
                };
            }
    }
}