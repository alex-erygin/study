open FsUnit
open Xunit
open MyProject.Customer

module ``When upgrading customer FS Unit`` =
    [<Fact>]
    let ``should give VIP customer more credit`` () =
        let customerVIP = { Id = 1; IsVip = true; Credit = 0.0M }
        let expected = { customerVIP with Credit = customerVIP.Credit + 100M }
        let actual = upgradeCustomer customerVIP
        actual |> should equal expected
        
