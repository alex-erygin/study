$store = New-Object -TypeName System.Security.Cryptography.X509Certificates.X509Store -ArgumentList @([System.Security.Cryptography.X509Certificates.StoreName]::AddressBook, [System.Security.Cryptography.X509Certificates.StoreLocation]::CurrentUser) 
$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
$store.Close()