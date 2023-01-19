Option Strict On
Option Explicit On
Imports SFML.System
Imports SFML.Window
Imports SFML.Graphics
Module Game
    ''' <summary>Routine of the game</summary>
    Public Sub Main()
        Const GridSize As Integer = 900
        Const Buffer As Integer = 10
        Const GridQTY As Integer = 30
        Const SquareSize As Integer = CInt(GridSize / GridQTY)

        'Set up game
        Randomize() 'initialize RNG
        'Dim Window As RenderWindow = InitializeWindow(GridSize, Buffer)
        InitializeGrid(GridSize, Buffer, Window)
        fillGrid(GridQTY, GridSize, SquareSize, Buffer, Window)
        Dim Snake As New Snake
        Snake.Initialize(GridQTY, Window, SquareSize, Buffer)
        Dim Fruit As New Fruit
        Fruit.Create(GridQTY, Window, SquareSize, Buffer, Snake.Body)

        Window.Display()
        Snake.Move(CInt(InputBox("Direction")), SquareSize, Snake, Window, Buffer, Fruit)

        Window.Display()
        MsgBox("Test")
    End Sub
    ''' <summary>The next location for the snake. 2nd rank is X,Y coordinates</summary>
    Public Function NextLocation(Direction As Integer, Existing(,) As Integer) As Integer(,)
        Select Case Direction
            Case 1 'up
                NextLocation = {{Existing(0, 0), Existing(0, 1) + 1}}
            Case 2 'down
                NextLocation = {{Existing(0, 0), Existing(0, 1) - 1}}
            Case 3 'left
                NextLocation = {{Existing(0, 0) - 1, Existing(0, 1)}}
            Case 4 'right
                NextLocation = {{Existing(0, 0) + 1, Existing(0, 1)}}
            Case Else 'no bueno
                NextLocation = Nothing
                MsgBox("ERROR - BAD DIRECTION")
        End Select
    End Function
    ''' <summary>Draws a square in the specified location</summary>
    Public Sub DrawSquare(X As Integer, Y As Integer, Color As Color, SquareSize As Integer, Window As RenderWindow, Buffer As Integer)
        Dim Size As Vector2f = New Vector2f(SquareSize - 1, SquareSize - 1)
        Dim Square As Shape = New RectangleShape(Size) With {
            .FillColor = Color,
            .OutlineColor = Color.Black,
            .OutlineThickness = 1,
            .Position = New Vector2f((X * SquareSize) + Buffer + 1, (Y * SquareSize) + Buffer + 1)
        }
        Window.Draw(Square)
    End Sub
End Module
Module Setup
    ''' <summary>Initialize game window</summary>
    Public Function InitializeWindow(GridSize As UInteger, Buffer As UInteger) As RenderWindow
        Dim Size As UInteger = CType(GridSize + Buffer * 2, UInteger)
        Dim Window As RenderWindow = New RenderWindow(New VideoMode(Size, Size), "Snake")

        Window.Display()

        InitializeWindow = Window
    End Function
    ''' <summary>Draw outline of grid</summary>
    Public Sub InitializeGrid(GridSize As UInteger, Buffer As Integer, Window As RenderWindow)
        Dim Size As Vector2f = New Vector2f(GridSize, GridSize)
        Dim Grid As Shape = New RectangleShape(Size) With {
            .FillColor = Color.Transparent,
            .OutlineColor = Color.White,
            .OutlineThickness = 3,
            .Position = New Vector2f(Buffer, Buffer)
        }
        Window.Draw(Grid)
    End Sub
    ''' <summary>Draw inside of grid</summary>
    Public Sub FillGrid(GridQTY As Integer, GridSize As Integer, SquareSize As Integer, Buffer As Integer, Window As RenderWindow)
        'Draw vertical lines
        Dim Line As Shape
        Dim LineSize As Vector2f = New Vector2f(1, GridSize)
        For Count = 1 To GridQTY - 1
            Line = New RectangleShape(LineSize) With {
            .FillColor = Color.Black,
            .Position = New Vector2f(Buffer + SquareSize * Count, Buffer)
            }
            Window.Draw(Line)
        Next

        'Draw horizontal lines
        LineSize = New Vector2f(GridSize, 1)
        For Count = 1 To GridQTY - 1
            Line = New RectangleShape(LineSize) With {
            .FillColor = Color.Black,
            .Position = New Vector2f(Buffer, Buffer + SquareSize * Count)
            }
            Window.Draw(Line)
        Next
    End Sub
End Module
Public Class Snake
    Property Length As Integer = 1
    Public Body(Length, 1) As Integer '2nd dimension is X,Y coordinate
    Public Color As Color = Color.Green
    Public Alive As Boolean = True
    ''' <summary>Start snake in a random position</summary>
    Public Sub Initialize(GridQTY As Integer, Window As RenderWindow, SquareSize As Integer, Buffer As Integer)
        'Determine random location
        Body(0, 0) = CInt(((GridQTY - 2) - 1) * Rnd() + 1)
        Body(0, 1) = CInt(((GridQTY - 2) - 1) * Rnd() + 1)

        'Determine random direction
        Dim Tail(,) As Integer = Game.NextLocation(CInt((4 - 1) * Rnd() + 1), Body)
        Body(1, 0) = Tail(0, 0)
        Body(1, 1) = Tail(0, 1)

        'Draw the snake
        DrawSquare(Body(0, 0), Body(0, 1), Color, SquareSize, Window, Buffer)
        DrawSquare(Body(1, 0), Body(1, 1), Color, SquareSize, Window, Buffer)
    End Sub
    ''' <summary>Moves the snake in the specified direction</summary>
    Public Sub Move(Direction As Integer, SquareSize As Integer, Snake As Snake, Window As RenderWindow, Buffer As Integer, Fruit As Fruit)
        Dim NewLocation As Integer(,) = Game.NextLocation(Direction, Body)

        If Fruit.Eaten Then

        End If

        DrawSquare(NewLocation(0, 0), NewLocation(0, 1), Color, SquareSize, Window, Buffer)
    End Sub
    ''' <summary>???Toggle for when the fruit is eaten and a length is added to the snake???</summary>
    Public Sub RemoveTail()

    End Sub
End Class
Public Class Fruit
    Public Location(0, 1) As Integer '2nd dimension is X,Y coordinate
    Public Color As Color = Color.Red
    Public Eaten As Boolean = False
    ''' <summary>Create the fruit in a random location</summary>
    Public Sub Create(GridQTY As Integer, Window As RenderWindow, SquareSize As Integer, Buffer As Integer, Snake(,) As Integer)
        'Determine random location
        Location(0, 0) = CInt(((GridQTY - 1)) * Rnd())
        Location(0, 1) = CInt(((GridQTY - 1)) * Rnd())

        'make sure it's not colliding with the snake
        For Count = LBound(Snake, 1) To UBound(Snake, 1)
            If Snake(Count, 0) = Location(0, 0) And Snake(Count, 1) = Location(0, 1) Then
                Create(GridQTY, Window, SquareSize, Buffer, Snake)
            End If
        Next
        Eaten = False

        'Draw the fruit
        DrawSquare(Location(0, 0), Location(0, 1), Color, SquareSize, Window, Buffer)

    End Sub
End Class