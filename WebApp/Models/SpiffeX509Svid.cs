using System;
using System.Security.Cryptography.X509Certificates;
using InspectorGadget.WebApp.Gadgets.Spiffe;

namespace InspectorGadget.WebApp.Infrastructure
{
    public class SpiffeX509Svid
    {
        public string SpiffeId { get; set; }
        public string Base64Certificate { get; set; } // Base64 of ASN.1 DER encoded certificate chain. MAY include intermediates, the leaf certificate (or SVID itself) MUST come first.
        public string Base64PrivateKey { get; set; } // Base64 of ASN.1 DER encoded PKCS#8 private key. MUST be unencrypted.
        public string Base64Bundle { get; set; } // Base64 of ASN.1 DER encoded X.509 bundle for the trust domain.
        public string Hint { get; set; }
        public string CertificateIssuer { get; set; }
        public string CertificateSubject { get; set; }
        public string CertificateSubjectAlternativeName { get; set; }
        public string CertificateThumbprint { get; set; }
        public string CertificateSerialNumber { get; set; }
        public DateTime? CertificateNotBeforeUtc { get; set; }
        public DateTime? CertificateNotAfterUtc { get; set; }

        public SpiffeX509Svid()
        {
        }

        public SpiffeX509Svid(X509SVID svid)
        {
            this.SpiffeId = svid.SpiffeId;
            this.Base64Certificate = svid.X509Svid.ToBase64();
            this.Base64PrivateKey = svid.X509SvidKey.ToBase64();
            this.Base64Bundle = svid.Bundle.ToBase64();
            this.Hint = svid.Hint;

            try
            {
                // Attempt to get more details from the actual certificate.
                var certificatePem = $"-----BEGIN CERTIFICATE-----{Environment.NewLine}{this.Base64Certificate}{Environment.NewLine}-----END CERTIFICATE-----";
                var privateKeyPem = $"-----BEGIN PRIVATE KEY-----{Environment.NewLine}{this.Base64PrivateKey}{Environment.NewLine}-----END PRIVATE KEY-----";
                var certificate = X509Certificate2.CreateFromPem(certificatePem, privateKeyPem);
                this.CertificateIssuer = certificate.Issuer;
                this.CertificateSubject = certificate.Subject;
                this.CertificateSubjectAlternativeName = certificate.GetNameInfo(X509NameType.UrlName, false);
                this.CertificateThumbprint = certificate.Thumbprint;
                this.CertificateSerialNumber = certificate.SerialNumber;
                this.CertificateNotBeforeUtc = certificate.NotBefore.ToUniversalTime();
                this.CertificateNotAfterUtc = certificate.NotAfter.ToUniversalTime();
            }
            catch
            {
                // Ignore errors.
            }
        }
    }
}