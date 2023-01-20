Option Explicit On
Option Strict On
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing

Public Class Game
    Private Shared Property Settings As Settings
    Private Property Network As Network
    Public Property Snake As Snake
    Public Property Fruit As Fruit
    Public Property Score As Integer
    Public Property Timer As Integer
    Public Property Clock As Integer
    Public Property Speed As Integer
    Public Sub New(Setting As Settings, NN As Network)
        Settings = Setting
        Network = NN

        Score = 0
        Timer = 500
        Clock = 0

        Snake = New Snake()
        Fruit = New Fruit()
        Do Until Not PointInSnake(Fruit.Location(0))
            Fruit = New Fruit()
        Loop
    End Sub
    Public Sub Run()
        Do Until Gameover
            Dim Stats As New List(Of Integer) From {
                FruitDistX,
                FruitDistY,
                WallDistPosX,
                WallDistNegX,
                WallDistPosY,
                WallDistNegY,
                BodyDistPosX,
                BodyDistNegX,
                BodyDistPosY,
                BodyDistNegY
            }
            Snake.Move(Network.Calculate(Stats))
            'Snake.Move(CInt(InputBox("Enter a direction" & vbCrLf &
            '                    "X Fruit: " & FruitDistX & vbCrLf &
            '                    "Y Fruit: " & FruitDistY & vbCrLf &
            '                    "X+ Wall: " & WallDistPosX & vbCrLf &
            '                    "Y+ Wall: " & WallDistPosY & vbCrLf &
            '                    "X- Wall: " & WallDistNegX & vbCrLf &
            '                    "Y- Wall: " & WallDistNegY & vbCrLf &
            '                    "X+ Snake: " & BodyDistPosX & vbCrLf &
            '                    "Y+ Snake: " & BodyDistPosY & vbCrLf &
            '                    "X- Snake: " & BodyDistNegX & vbCrLf &
            '                    "Y- Snake: " & BodyDistNegY & vbCrLf &
            '                    "Score: " & Score & vbCrLf &
            '                    "Timer: " & Timer & vbCrLf &
            '                    "Clock: " & Clock)))
            If SnakeInFood() Then
                Score += 1
                Speed = Clock
                Timer += Settings.TimerBump
                Snake.Grow()
                Fruit = New Fruit()
                Do Until Not PointInSnake(Fruit.Location(0))
                    Fruit = New Fruit()
                Loop
            End If
            Clock += 1
            Timer -= 1
            If Timer > Settings.TimerLimit Then
                Timer = Settings.TimerLimit
            End If
            UpdateUI()
        Loop
        Network.Fitness.Add(Fitness)
        'MsgBox("Game Over")
    End Sub
    ''' <summary> Get a random integer point within the bounds of the grid </summary>
    Public Shared Function RNGGrid(Optional Offset As Integer = 0) As Point
        Return New Point(Random.Next(0 + Offset, Settings.GridSquares - Offset), Random.Next(0 + Offset, Settings.GridSquares - Offset))
    End Function
    Public Shared Function GetNewPoint(Direction As Integer, Point As Point) As Point
        Select Case Direction
            Case 1 'N
                Point.Y -= 1
            Case 2 'E
                Point.X += 1
            Case 3 'S
                Point.Y += 1
            Case 4 'W
                Point.X -= 1
        End Select
        Return Point
    End Function
    Public Function PointInSnake(Point As Point, Optional SkipHead As Boolean = False) As Boolean
        For Each Segment As Point In Snake.Location
            If SkipHead = True Then
                SkipHead = False
                Continue For
            End If
            If Point = Segment Then
                Return True
            End If
        Next
        Return False
    End Function
    Public ReadOnly Property PointInBounds(Point As Point) As Boolean
        Get
            If Point.X < 0 Or Point.Y < 0 Or Point.X >= Settings.GridSquares Or Point.Y >= Settings.GridSquares Then
                Return False
            End If
            Return True
        End Get
    End Property
    Public ReadOnly Property SnakeInFood() As Boolean
        Get
            Return Snake.Head = Fruit.Location(0)
        End Get
    End Property
    Public ReadOnly Property Gameover() As Boolean
        Get
            If PointInSnake(Snake.Head, True) Or Snake.TurnedAround Or Not PointInBounds(Snake.Head) Or Timer <= 0 Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
    Public ReadOnly Property FruitDistX() As Integer
        Get
            Return Snake.Head.X - Fruit.Location(0).X
        End Get
    End Property
    Public ReadOnly Property FruitDistY() As Integer
        Get
            Return Snake.Head.Y - Fruit.Location(0).Y
        End Get
    End Property
    Public ReadOnly Property WallDistNegX() As Integer
        Get
            Return Snake.Head.X
        End Get
    End Property
    Public ReadOnly Property WallDistPosX() As Integer
        Get
            Return Settings.GridSquares - Snake.Head.X
        End Get
    End Property
    Public ReadOnly Property WallDistNegY() As Integer
        Get
            Return Snake.Head.Y
        End Get
    End Property
    Public ReadOnly Property WallDistPosY() As Integer
        Get
            Return Settings.GridSquares - Snake.Head.Y
        End Get
    End Property
    Public ReadOnly Property BodyDistPosX() As Integer
        Get
            For Each Segment As Point In Snake.Location
                If Segment.X > Snake.Head.X Then
                    If Segment.Y = Snake.Head.Y Then
                        Return Segment.X - Snake.Head.X
                    End If
                End If
            Next
            Return WallDistPosX
        End Get
    End Property
    Public ReadOnly Property BodyDistNegX() As Integer
        Get
            For Each Segment As Point In Snake.Location
                If Segment.X < Snake.Head.X Then
                    If Segment.Y = Snake.Head.Y Then
                        Return Snake.Head.X - Segment.X
                    End If
                End If
            Next
            Return WallDistNegX
        End Get
    End Property
    Public ReadOnly Property BodyDistPosY() As Integer
        Get
            For Each Segment As Point In Snake.Location
                If Segment.Y > Snake.Head.Y Then
                    If Segment.X = Snake.Head.X Then
                        Return Segment.Y - Snake.Head.Y
                    End If
                End If
            Next
            Return WallDistPosY
        End Get
    End Property
    Public ReadOnly Property BodyDistNegY() As Integer
        Get
            For Each Segment As Point In Snake.Location
                If Segment.Y < Snake.Head.Y Then
                    If Segment.X = Snake.Head.X Then
                        Return Snake.Head.Y - Segment.Y
                    End If
                End If
            Next
            Return WallDistNegY
        End Get
    End Property
    Public ReadOnly Property Fitness As Double
        Get
            If Score = 0 Or Speed = 0 Then Return 0
            Return Score + (1 / Speed)
        End Get
    End Property
End Class

Public MustInherit Class GameElement
    Public Property Location As List(Of Point)
    Public MustOverride ReadOnly Property Color As SFML.Graphics.Color
End Class

Public Class Snake
    Inherits GameElement
    Public Sub New()
        Location = New List(Of Point)
        Location.Add(Game.RNGGrid(1))
        Location.Add(Game.GetNewPoint(Random.Next(1, 4), Head))
    End Sub
    Public Property DeletedPoint As Point
    Public Property LastNeck As Point
    Public Sub Move(Direction As Integer)
        LastNeck = Location(1)
        Location.Insert(0, Game.GetNewPoint(Direction, Head))
        DeletedPoint = Location(Location.Count - 1)
        Location.RemoveAt(Location.Count - 1)
    End Sub
    Public Sub Grow()
        Location.Add(DeletedPoint)
    End Sub
    Public Overrides ReadOnly Property Color As SFML.Graphics.Color
        Get
            Return SFML.Graphics.Color.Green
        End Get
    End Property
    Public ReadOnly Property Head As Point
        Get
            Return Location.Item(0)
        End Get
    End Property
    Public ReadOnly Property TurnedAround As Boolean
        Get
            If Head = LastNeck Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property
End Class

Public Class Fruit
    Inherits GameElement
    Public Sub New()
        Dim Point As Point = Game.RNGGrid()
        Location = New List(Of Point) From {
            Game.RNGGrid()
        }
    End Sub
    Public Overrides ReadOnly Property Color As SFML.Graphics.Color
        Get
            Return SFML.Graphics.Color.Red
        End Get
    End Property
End Class