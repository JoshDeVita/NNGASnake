Option Strict On
Option Explicit On
Imports System.Collections.Generic
Imports System.Text.Json
Public Module Application
	Public Settings As Settings
	Public Display As Display
	Public Random As Random
	Public Game As Game
	Public Sub Main()
		Random = New Random
		Settings = New Settings
		Display = New Display(Settings)
		Dim Population As New Population(Settings)
		Game = New Game(Settings, Population.Networks.Item(0))
		UpdateUI()
		Game.Run()
	End Sub
	Public Function RNG(LowerBound As Double, UpperBound As Double) As Double
		Return Random.NextDouble * (UpperBound - LowerBound) + LowerBound
	End Function
	Public Function SerializeNetwork(Network As Network) As String
		Return JsonSerializer.Serialize(Network)
	End Function
	Public Function DeserializeNetwork(JSON As String) As Network
		Return JsonSerializer.Deserialize(Of Network)(JSON)
	End Function
	Public Sub SaveNetwork(File As String, Network As Network)
		Dim swLog As New IO.StreamWriter(File, True)
		swLog.WriteLine(SerializeNetwork(Network))
		swLog.Close()
	End Sub
	Public Sub UpdateUI() 'Draw and update the UI for every frame
		If Settings.Visible = False Then Exit Sub
		Display.Clear()
		'Draw grid
		Display.DrawGrid()
		'Draw snake
		For Each Point In Game.Snake.Location
			Display.DrawSquare(Point, Game.Snake.Color)
		Next
		'Draw fruit
		For Each Point In Game.Fruit.Location
			Display.DrawSquare(Point, Game.Fruit.Color)
		Next
		Display.Update()
	End Sub
End Module
Public Class Settings
	Public Property Visible As Boolean = True
	Public Property WindowSize As UInteger = 1200
	Public Property Buffer As Integer = 10
	Public Property GridSquares As Integer = 30
	Public Property FolderPath As String = "C:\Code\Networks\"
	Public Property PopulationSize As Integer = 100
	Public Property TimerLimit As Integer = 10000
	Public Property TimerBump As Integer = 100
End Class