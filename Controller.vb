Option Strict On
Option Explicit On
Imports System.Collections.Generic
Imports System.Text.Json
Imports Microsoft.VisualBasic.Devices

Public Module GlobalVariables
	Public Settings As New Setting
End Module

Public Module Application
	Public Display As Display
	Public Random As Random
	Public Game As Game
	Public Sub Main()
		Random = New Random
		Display = New Display
		Dim Population As Population
		Dim File As String = Nothing
		If Settings.Visible Then
			File = TestPopulation()
		End If
		If Settings.Visible AndAlso File <> "" Then 'Run a specific network for display
			Population = LoadPopulation(File)
			For i = 1 To Settings.PlayTop
				Game = New Game(Population.Networks.Item(i - 1))
				UpdateUI()
				Game.Run()
			Next
		Else 'Run normally
			Population = New Population(True)
			Dim LastGen As Population = Nothing
			For i = 1 To Settings.Generations
				If LastGen IsNot Nothing Then
					Population = New Population(False, LastGen)
				End If
				For Each Network In Population.Networks
					For j = 1 To Settings.Tests
						Game = New Game(Network)
						UpdateUI()
						Game.Run()
					Next
					LastGen = Population
				Next
				SavePopulation(Population, i)
			Next
		End If
	End Sub
	Public Function TestPopulation() As String
		Dim FileDialog As New Windows.Forms.OpenFileDialog
		FileDialog.InitialDirectory = Settings.FolderPath
		FileDialog.ShowDialog()
		Return FileDialog.FileName
	End Function
	Public Function RNG(LowerBound As Double, UpperBound As Double) As Double
		Return Random.NextDouble * (UpperBound - LowerBound) + LowerBound
	End Function
	Public Function RNGInt(LowerBound As Integer, UpperBound As Integer) As Integer
		Return Random.Next(LowerBound, UpperBound)
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
	Public Sub SavePopulation(Population As Population, Generation As Integer)
		Population.Sort()
		Dim swLog As New IO.StreamWriter(Settings.FolderPath & "Gen " & Generation & ".txt", False)
		swLog.Close()

		For Each Network In Population.Networks
			SaveNetwork(Settings.FolderPath & "Gen " & Generation & ".txt", Network)
		Next
	End Sub
	Public Function LoadPopulation(File As String) As Population
		Dim srLog As New IO.StreamReader(File)
		Dim Pop As New Population(False)
		For i = 1 To Settings.PlayTop
			Pop.Networks.Add(DeserializeNetwork(srLog.ReadLine))
		Next
		Return Pop
	End Function
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
Public Class Setting
	Public Property FolderPath As String = "C:\Code\Networks\"
	Public Property Visible As Boolean = True
	Public Property PlayTop As Integer = 10

	Public Property WindowSize As UInteger = 1200
	Public Property Buffer As Integer = 10
	Public Property GridSquares As Integer = 30
	Public Property FrameRate As Integer = 15

	Public Property TimerLimit As Integer = 5000
	Public Property TimerBump As Integer = 100

	Public Property RNGBounds As Integer = 5
	Public Property LayerQTY As Integer = 2
	Public Property NeuronQTY As Integer = 6

	Public Property Mutate As Boolean = True
	Public Property MutatePercent As Double = 0.02
	Public Property GeneMutatePercent As Double = 0.03

	Public Property Crossover As Boolean = True
	Public Property CrossoverType As String = "Point" 'Uniform, Point
	Public Property FitCrossover As Double = 0.25
	Public Property RandomCrossover As Double = 0.05
	Public Property CrossoverPercent As Double = 0.5

	Public Property DropZeros As Boolean = True
	Public Property PopulationSize As Integer = 100000
	Public Property Generations As Integer = 10
	Public Property Tests As Integer = 10


End Class