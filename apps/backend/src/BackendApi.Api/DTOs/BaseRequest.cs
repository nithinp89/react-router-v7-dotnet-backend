using System.ComponentModel.DataAnnotations;

namespace BackendApi.Api.DTOs
{
    /// <summary>
    /// Base request DTO containing CorrelationId from header.
    /// </summary>
    public class BaseRequest
    {
        /// <summary>
        /// Correlation ID for tracing requests across services, populated from the 'x-correlation-id' header if present.
        /// </summary>
        //[FromHeader(headerName: "x-correlation-id", isRequired: false)]
        //public string CorrelationId { get; set; } = string.Empty;
    }
}
