module Main exposing (..)

import Data exposing (..)
import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import List exposing (..)
import Types exposing (..)


type alias Model =
    { conflictList : List Conflict
    , selectedList : List Conflict
    , selectedConflict : Maybe Conflict
    , selectedFamilyMember : FamilyMember
    , searchString : String
    }


type FamilyMember
    = All
    | Sr
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
    , selectedFamilyMember = All
    , searchString = ""
    }



-- update


type Msg
    = Search String
    | SelectConflict Conflict
    | ChooseFamilyMember FamilyMember
    | Clear


update : Msg -> Model -> Model
update msg model =
    case msg of
        Search search ->
            filterConflicts { model | searchString = search }

        SelectConflict conflict ->
            { model | selectedConflict = Just conflict }

        ChooseFamilyMember selectedFamilyMember ->
            filterConflicts { model | selectedFamilyMember = selectedFamilyMember }

        Clear ->
            filterConflicts { model | searchString = "", selectedFamilyMember = All }


filterConflicts : Model -> Model
filterConflicts model =
    filterBySearch model
        |> filterByFamilyMember



clearFilter : Model -> Model
clearFilter model =
    model



-- This begins with all conflicts - i.e. conflictList, so must come first


filterBySearch : Model -> Model
filterBySearch model =
    { model | selectedList = List.filter (\record -> String.contains (String.toUpper model.searchString) (String.toUpper (record.conflictingEntity ++ record.description))) model.conflictList }



-- This begins with selectedList, so expects some filtering has already happened


filterByFamilyMember : Model -> Model
filterByFamilyMember model =
    case model.selectedFamilyMember of
        All ->
            model

        _ ->
            { model | selectedList = List.filter (\record -> stringToFamilyMember record.familyMember == model.selectedFamilyMember) model.selectedList }



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
        [ style [ ( "width", "100%" ) ] ]
        [ tr []
            [ td [ style [ ( "font-size", "xx-large" ) ] ] [ text "Tracking Trump's Conflicts of Interest" ]
            ]
        , tr []
            [ td []
                [ input [ type_ "text", placeholder "Search", value model.searchString, onInput Search ] []
                , td [] [ text (toString (length model.selectedList) ++ " conflicts") ]
                , td [] [ button [ onClick Clear ] [ text "Clear" ] ]

                --, td [] [ fieldset [] (List.map familyMemberChooser [ All, Sr, Jr, Ivanka, Jared, Melania, Eric ]) ]
                , td []
                    [ fieldset []
                        [ familyMemberRadio "All" (model.selectedFamilyMember == All) (ChooseFamilyMember All)
                        , familyMemberRadio "Trump" (model.selectedFamilyMember == Sr) (ChooseFamilyMember Sr)
                        , familyMemberRadio "Jr" (model.selectedFamilyMember == Jr) (ChooseFamilyMember Jr)
                        , familyMemberRadio "Ivanka" (model.selectedFamilyMember == Ivanka) (ChooseFamilyMember Ivanka)
                        , familyMemberRadio "Jared" (model.selectedFamilyMember == Jared) (ChooseFamilyMember Jared)
                        , familyMemberRadio "Melania" (model.selectedFamilyMember == Melania) (ChooseFamilyMember Melania)
                        , familyMemberRadio "Eric" (model.selectedFamilyMember == Eric) (ChooseFamilyMember Eric)
                        ]
                    ]
                ]
            ]
        , tr []
            [ td [ conflictPaneStyle ] (drawConflictRows model.selectedList model.selectedConflict)
            , td [ sourcePaneStyle ] (drawSources model.selectedConflict)
            ]
        ]


drawConflictRows : List Conflict -> Maybe Conflict -> List (Html Msg)
drawConflictRows conflicts selectedConflict =
    let
        drawConflictRow : Conflict -> Html Msg
        drawConflictRow conflict =
            tr [ onClick (SelectConflict conflict) ]
                [ td [ style [ ( "width", "70px" ) ], classList [ ( "selected", isSelected selectedConflict conflict ) ] ] [ text conflict.familyMember ]
                , td [ style [ ( "width", "70px" ) ], classList [ ( "selected", isSelected selectedConflict conflict ) ] ] [ text conflict.category ]
                , td [ style [ ( "width", "100px" ) ], classList [ ( "selected", isSelected selectedConflict conflict ) ] ] [ text conflict.conflictingEntity ]
                , td [ style [ ( "width", "400px" ) ], classList [ ( "selected", isSelected selectedConflict conflict ) ] ] [ text conflict.description ]
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
                [ td [ style [ ( "width", "150px" ) ] ] [ a [ href source.link ] [ text source.name ] ]
                , td [ style [ ( "width", "80px" ) ] ] [ text source.date ]
                ]
    in
    case conflict of
        Nothing ->
            [ h3 [] [ text <| "" ] ]

        Just conflict ->
            conflict.sources
                |> List.map drawSourceRow


familyMemberRadio : String -> Bool -> msg -> Html msg
familyMemberRadio value isChecked msg =
    label
        [ style [ ( "padding", "20px" ) ]
        ]
        [ input [ type_ "radio", name "familyMember", onClick msg, checked isChecked ] []
        , text value
        ]


familyMemberToString : FamilyMember -> String
familyMemberToString person =
    case person of
        All ->
            "All"

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
        "All" ->
            All

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
