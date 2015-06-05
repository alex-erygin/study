using System;
using System.Security.Cryptography;

namespace НарlochromisBurtoni.Security.Cryptography
{
    /// <summary>
    ///     The CngProvider2 class provides additional <see cref="CngProvider" /> objects to suppliment the
    ///     ones found on the standard <see cref="CngProvider" /> type.
    /// </summary>
    public class CngProvider2 : IEquatable<CngProvider2>
    {
        /// <summary>
        /// Microsoft Primitive Provider (MS_PRIMITIVE_PROVIDER).
        /// </summary>
        private const string MicrosoftPrimitiveProvider = "Microsoft Primitive Provider";

        private readonly CngProvider provider;
        private static CngProvider2 microsoftPrimitiveAlgorithmProvider;

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="CngProvider2" />
        ///     по строковому идентификатору провайдера.
        /// </summary>
        /// <param name="provider">Идентификатор провайдера.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="provider"/> null.</exception>
        public CngProvider2(string provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this.provider = new CngProvider(provider);
        }

        /// <summary>
        ///     Инициализирует новый экземпляр класса <see cref="CngProvider2" />
        ///     по экземпляру системного <see cref="CngProvider" />.
        /// </summary>
        /// <param name="provider">
        ///     Экземпляр класса <see cref="CngProvider" />, представляющий провайдер.
        /// </param>
        /// <exception cref="ArgumentNullException">Если <paramref name="provider"/> null.</exception>
        public CngProvider2(CngProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            this.provider = provider;
        }

        /// <summary>
        /// Get a CngProvider2 for the
        /// Microsoft Primitive Provider (MS_PRIMITIVE_PROVIDER).
        /// </summary>
        public static CngProvider2 MicrosoftPrimitiveAlgorithmProvider
        {
            get
            {
                if (microsoftPrimitiveAlgorithmProvider == null)
                {
                    microsoftPrimitiveAlgorithmProvider = new CngProvider2(MicrosoftPrimitiveProvider);
                }

                return microsoftPrimitiveAlgorithmProvider;
            }
        }

        /// <summary>
        ///     Получить хранимый экземпляр объекта <see cref="CngProvider" />.
        /// </summary>
        /// <returns>
        ///     Хранимый объект <see cref="CngProvider" />.
        /// </returns>
        public CngProvider CngProvider
        {
            get { return provider; }
        }

        /// <summary>
        ///     Получить идентификатор хранимого провайдера.
        /// </summary>
        /// <returns>
        ///     Идентификатор хранимого провайдера.
        /// </returns>
        public string Provider
        {
            get { return CngProvider.Provider; }
        }

        /// <summary>
        ///     Determines whether two <see cref="CngProvider2" /> objects specify the same provider.
        /// </summary>
        /// <returns>
        ///     true if the two objects represent the same provider; otherwise, false.
        /// </returns>
        /// <param name="left">An object that specifies a provider.</param>
        /// <param name="right">
        ///     A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.
        /// </param>
        public static bool operator ==(CngProvider2 left, CngProvider2 right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            else
            {
                return left.Equals(right);
            }
        }

        /// <summary>
        ///     Determines whether two <see cref="CngProvider2" /> objects do not represent the same provider.
        /// </summary>
        /// <returns>
        ///     true if the two objects do not represent the same provider; otherwise, false.
        /// </returns>
        /// <param name="left">An object that specifies a provider.</param>
        /// <param name="right">
        ///     A second object, to be compared to the object that is identified by the <paramref name="left" /> parameter.
        /// </param>
        public static bool operator !=(CngProvider2 left, CngProvider2 right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            else
            {
                return !left.Equals(right);
            }
        }

        /// <summary>
        ///     Compares the specified object to the current <see cref="CngProvider2" /> object.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="obj" /> parameter is a <see cref="CngProvider2" /> that specifies
        ///     the same provider as the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">
        ///     An object to be compared to the current <see cref="CngProvider2" /> object.
        /// </param>
        public override bool Equals(object obj)
        {
            return Equals(obj as CngProvider2);
        }

        /// <summary>
        ///     Generates a hash value for the name of the provider
        ///     that is embedded in the current <see cref="CngProvider2" /> object.
        /// </summary>
        /// <returns>
        ///     The hash value of the embedded provider name.
        /// </returns>
        public override int GetHashCode()
        {
            return provider.GetHashCode();
        }

        /// <summary>
        ///     Gets the name of the provider that the current <see cref="CngProvider2" /> object specifies.
        /// </summary>
        /// <returns>
        ///     The embedded provider name.
        /// </returns>
        public override string ToString()
        {
            return provider.ToString();
        }

        /// <summary>
        ///     Compares the specified <see cref="CngProvider2" /> object to the current <see cref="CngProvider2" /> object.
        /// </summary>
        /// <returns>
        ///     true if the <paramref name="other" /> parameter specifies the same provider as the current object;
        ///     otherwise, false.
        /// </returns>
        /// <param name="other">
        ///     An object to be compared to the current <see cref="CngProvider2" /> object.
        /// </param>
        public bool Equals(CngProvider2 other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return provider.Equals(other.provider);
            }
        }
    }
}
