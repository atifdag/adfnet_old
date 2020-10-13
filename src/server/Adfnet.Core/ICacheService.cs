using System.Collections.Generic;

namespace Adfnet.Core
{
    /// <summary>
    /// Önbellekleme için arayüz
    /// </summary>
    public interface ICacheService
    {

        /// <summary>
        /// Önbellekteki değeri getirir.
        /// </summary>
        /// <param name="key">Anahtar</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Önbellekteki değeri istenilen türde döndürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Anahtar</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// Önbellekte olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="key">Anahtar</param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// Önbelleğe değer ekler
        /// </summary>
        /// <param name="key">Anahtar</param>
        /// <param name="value">Değer</param>
        void Add(string key, object value);

        /// <summary>
        /// Önbellekteki kaydı siler.
        /// </summary>
        /// <param name="key">Anahtar</param>
        void Remove(string key);

        List<string> GetAllKeyList();

        void AddToKeyList(string key);

        void RemoveFromKeyList(string key);

        void CleanKeyList();


    }
}
