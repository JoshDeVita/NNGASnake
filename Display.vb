Option Strict On
Option Explicit On
Imports SFML.System
Imports SFML.Window
Imports SFML.Graphics
Imports System.Collections.Generic

Public Class Display
    Private Property Window As RenderWindow
    Private Property Grid As List(Of RectangleShape)
    Private Property Settings As Settings
    Public Sub New(Setting As Settings)
        Settings = Setting
        If Settings.Visible Then
            Initialize()
        End If
    End Sub
    Private Sub initialize()
        initializeWindow()

        Update()
    End Sub
    Public Sub Clear()
        Window.Clear()
    End Sub
    Public Sub Update()
        Window.Display()
    End Sub
    Private Sub initializeWindow()
        Dim Length As UInteger = CUInt(Settings.WindowSize + Settings.Buffer * 2)
        Window = New RenderWindow(New VideoMode(Length, Length), "Snake")
    End Sub
    Public Sub DrawGrid()
        drawRectangle(Settings.WindowSize, Settings.WindowSize, Color.White, Color.Transparent, Settings.Buffer, Settings.Buffer, 3)

        'Draw all inner lines
        Grid = New List(Of RectangleShape)
        For i = 1 To Settings.GridSquares
            drawRectangle(1, Settings.WindowSize, Color.Transparent, Color.White, Settings.Buffer + SquareSize * i, Settings.Buffer) 'vertical lines
            drawRectangle(Settings.WindowSize, 1, Color.Transparent, Color.White, Settings.Buffer, Settings.Buffer + SquareSize * i) 'horizontal lines
        Next

    End Sub
    Private Sub drawRectangle(Width As Single, Height As Single, Outline As Color, Fill As Color, X As Single, Y As Single, Optional Thickness As Single = 1)
        Dim Vector As New Vector2f(Width, Height)
        Dim Shape As Shape = New RectangleShape(Vector) With {
            .FillColor = Fill,
            .OutlineColor = Outline,
            .OutlineThickness = Thickness,
            .Position = New Vector2f(X, Y)
        }
        Window.Draw(Shape)
    End Sub
    Public Sub DrawSquare(Point As Drawing.Point, Color As Color)
        drawRectangle(SquareSize - 1, SquareSize - 1, Color.Transparent, Color, (Point.X * SquareSize) + Settings.Buffer + 1, (Point.Y * SquareSize) + Settings.Buffer + 1)
    End Sub
    Private ReadOnly Property SquareSize As Integer
        Get
            Return CInt(Settings.WindowSize / Settings.GridSquares)
        End Get
    End Property
End Class