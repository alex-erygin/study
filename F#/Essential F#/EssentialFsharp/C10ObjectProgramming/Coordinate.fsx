// Equality

// Most of the types in F# support structural equality but class types do not. Instead, they rely on
// reference equality like most things in .NET. Structural equality is when two things contain the same
// data. Reference equality is when two things point to the same underlying instance.

type Coordinate(latitude: float, longtitude: float) =
    member _.Latitude = latitude
    member _.longitude = longtitude

let c1 = Coordinate(25.0, 11.98)
let c2 = Coordinate(25.0, 11.98)
let c3 = c1
c1 = c2 // false
c1 = c3 // true - reference the same instance

// To support something that works like structural equality, we need to override the GetHashCode and
// Equals functions, implement IEquatable<'T>. If we are going to use it in other .NET languages, we
// need to handle the equality operator using op_Equality and apply the AllowNullLiteral attribute
open System

[<AllowNullLiteral>]
type GpsCoordinate(latitude: float, longitude: float) =
    let equals (other: GpsCoordinate) =
        if isNull other then
            false
        else
            latitude = other.Latitude
            && longitude = other.Longitude
            
    member _.Latitude = latitude
    member _.Longitude = longitude
    
    override this.GetHashCode() =
        hash (this.Latitude, this.Longitude)
        override _.Equals(obj) =
        match obj with
            | :? GpsCoordinate as other -> equals other
            | _ -> false
    
    interface IEquatable<GpsCoordinate> with
        member _.Equals(other: GpsCoordinate) =
            equals other

    static member op_Equality(this: GpsCoordinate, other: GpsCoordinate) =
        this.Equals(other)

let c11 = GpsCoordinate(25.0, 11.98)
let c22 = GpsCoordinate(25.0, 11.98)
c11 = c22 // true
