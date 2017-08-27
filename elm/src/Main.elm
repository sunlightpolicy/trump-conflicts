module Main exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import List exposing (..)

import Types exposing (..)
import Data exposing (..)


type alias Model =
    { conflictList : List Conflict
    , selectedList : List Conflict
    , selectedConflict : Maybe Conflict    
    , selectedFamilyMember : FamilyMember
    , searchString : String
    }

type FamilyMember
    = Sr 
    | Jr
    | Ivanka
    | Jared
    | Melania
    | Eric
 

initModel : Model
initModel = 
    { conflictList = conflictList
    , selectedList = conflictList
    , selectedConflict = Nothing
    , selectedFamilyMember = Sr
    , searchString = "" 
    }


-- update

type Msg
    = Search String
    | SelectConflict Conflict
    | ChooseFamilyMember FamilyMember


update : Msg -> Model -> Model
update msg model =
    case msg of
        Search search ->
            filterConflicts { model | searchString = search}

        SelectConflict conflict ->
            { model | selectedConflict = Just conflict} 

        ChooseFamilyMember selectedFamilyMember ->
            filterConflicts { model | selectedFamilyMember = selectedFamilyMember }             


filterConflicts : Model -> Model
filterConflicts model =
    { model | selectedList = List.filter (\record -> String.contains (String.toUpper model.searchString) (String.toUpper (record.conflictingEntity ++ record.description))) model.conflictList }
    -- { model | selectedList =  (filterByFamilyMember model).selectedList }


filterByFamilyMember : Model -> List Conflict
filterByFamilyMember model =
    List.filter (\record -> model.selectedFamilyMember == record.familyMember) model.conflictList
    


-- view

conflictPaneStyle : Attribute msg
conflictPaneStyle =
    style
        [ ( "float", "left" )
        ]

sourcePaneStyle : Attribute msg
sourcePaneStyle =
    style
        [ ( "float", "left" )
        , ( "width", "380px" )
        ]


view : Model -> Html Msg
view model =
    table
        [ style[ ( "width", "100%" ) ] ]
        [
            tr []
                [td [style[ ( "font-size", "xx-large" ) ]] [text "Tracking Trump's Conflicts of Interest"]
            ]
            ,tr []
                [ td [] [ input [type_ "text", placeholder "Search", onInput Search] []
                , td [] (List.map familyMemberChooser [ Sr, Jr, Ivanka, Jared, Melania, Eric ]) 
                ]
            ]
            ,tr [] 
                [ td [ conflictPaneStyle ] (drawConflictRows model.selectedList model.selectedConflict)
                , td [ sourcePaneStyle ] (drawSources (model.selectedConflict))
            ]
        ]
    

drawConflictRows : List Conflict -> Maybe Conflict -> List (Html Msg)
drawConflictRows conflicts selectedConflict =
    let 
        drawConflictRow : Conflict -> Html Msg
        drawConflictRow conflict = 
            tr [ onClick (SelectConflict conflict)] 
            [ td [style[ ( "width", "70px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.familyMember) ] 
            , td [style[ ( "width", "70px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.category) ]
            , td [style[ ( "width", "100px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.conflictingEntity) ] 
            , td [style[ ( "width", "400px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.description) ]
            ]
    in
        conflicts
            |> List.map drawConflictRow


isSelected : Maybe Conflict -> Conflict -> Bool
isSelected selectedConflict conflict = 
    case selectedConflict of
        Nothing ->
            False 

        Just selected ->
            selected == conflict     



drawSources : Maybe Conflict -> List (Html Msg)
drawSources conflict =
    let
        drawSourceRow : Source -> Html Msg
        drawSourceRow source = 
            tr [] 
            [ td [style[ ( "width", "150px" ) ]] [ a [ href source.link ] [text source.name ] ] 
            , td [style[ ( "width", "80px" ) ]] [text (source.date) ] 
            ]
    in
        case conflict of
            Nothing ->
                [ h3 [] [text <| ""] ]

            Just conflict ->
                conflict.sources
                    |> List.map drawSourceRow


familyMemberChooser : FamilyMember -> Html Msg  
familyMemberChooser person =
    label [] 
       -- [ input [ type_ "radio", name "familyMember", onClick (ChooseFamilyMember (stringToFamilyMember "familyMember")) ] []
       [ input [ type_ "radio", name "familyMember", onClick (ChooseFamilyMember person) ] []
        , text (familyMemberToString person)
        ]


familyMemberToString : FamilyMember -> String
familyMemberToString person =
    case person of
        Sr -> 
            "Trump" 
        Jr ->
            "Junior"
        Ivanka ->
            "Ivanka"
        Jared ->
            "Jared"
        Melania ->
            "Melania"
        Eric ->
            "Eric"

stringToFamilyMember : String -> FamilyMember
stringToFamilyMember person =
    case person of
        "Trump" -> 
            Sr 
        "Junior" ->
            Jr
        "Ivanka" ->
            Ivanka
        "Jared" ->
            Jared
        "Melania" ->
            Melania
        "Eric" ->
            Eric   
        _ ->
            Sr            


main : Program Never Model Msg
main =
    Html.beginnerProgram
        { model = initModel
        , view = view
        , update = update
        }

