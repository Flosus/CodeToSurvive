namespace CodeToSurvive.App.Public

open System

module PublicModel =
    type LoginModel = { ID: Guid; IsLoggedIn: bool }
