using ErpSystem.Application.Common.Interfaces;

namespace ErpSystem.Infrastructure.Services;

/// <summary>
/// Implementación concreta del proveedor de fecha y hora.
/// Envuelve DateTime.UtcNow en una interfaz para permitir que los tests
/// puedan inyectar fechas fijas y hacer las pruebas deterministas.
/// Sin esta abstracción, cualquier código que use DateTime.UtcNow directamente
/// es imposible de testear de forma confiable.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Retorna la fecha y hora actual en UTC.
    /// Siempre usar UTC en aplicaciones distribuidas para evitar
    /// problemas de zonas horarias entre servidores y clientes.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;
}