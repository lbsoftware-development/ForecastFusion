namespace ForecastFusion.Application.Caching
{
    public interface ICacheService
    {
        void SetValue(string key, object value);

        object GetValue(string key);

        void RemoveValue(string key);
    }
}
