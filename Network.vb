Option Explicit On
Option Strict On
Imports System.Collections.Generic
Public Class Population
	Public Property Networks As New List(Of Network)
	Public Sub New(Fresh As Boolean, Optional LastGen As Population = Nothing)
		If Fresh Then
			Initialize()
		End If

		If LastGen IsNot Nothing Then
			CreateNewGeneration(LastGen)
		End If

	End Sub
	Public Sub Initialize()
		For i = 1 To Settings.PopulationSize
			Dim Network As New Network
			Network.Initialize()
			Networks.Add(Network)
		Next
	End Sub
	Public Sub CreateNewGeneration(LastGen As Population)
		LastGen.Sort()
		Dim Parents As New List(Of Network)

		'Select top percentage
		Dim TopQTY As Integer = CInt(Settings.FitCrossover * Settings.PopulationSize)
		For i = 0 To TopQTY - 1
			Parents.Add(LastGen.Networks.Item(i))
		Next

		'Select random percentage
		Dim RandomQTY As Integer = CInt(Settings.RandomCrossover * Settings.PopulationSize)
		Dim RandomList As New List(Of Integer)
		Dim RandomTest As Integer
		For i = 0 To RandomQTY - 1
			Do
				RandomTest = RNGInt(TopQTY, Settings.PopulationSize - 1)
			Loop Until Not RandomList.Contains(RandomTest) 'Ensure that the same network is not selected twice
			Parents.Add(LastGen.Networks.Item(RandomTest))
		Next

		'Randomly crossover
		Do Until Networks.Count = Settings.PopulationSize
			Dim Parent1 As Network = Parents.Item(RNGInt(0, Parents.Count - 1))
			Dim Parent2 As Network = Parents.Item(RNGInt(0, Parents.Count - 1))
			If Parent1 Is Parent2 Then
				Continue Do
			End If
			Dim Children As List(Of Network) = Parent1.Crossover(Parent2)
			Networks.Add(Children.Item(0))
			Networks.Add(Children.Item(1))
		Loop

		'Randomly mutate
		For Each Network In Networks
			If RNG(0, 1) < Settings.MutatePercent Then
				Network.Mutate()
			End If
		Next
	End Sub
	Public Sub Sort()
		Networks.Sort(Function(x, y) x.AverageFitness.CompareTo(y.AverageFitness))
		Networks.Reverse()
	End Sub
End Class
Public Class Network
	Private ReadOnly Property Inputs As Integer = 10
	Private ReadOnly Property Outputs As Integer = 4
	Private ReadOnly Property LayerQTY As Integer
	Private ReadOnly Property NeuronQTY As Integer
	Public Property Neurons As New List(Of Neuron)
	Public Property Synapses As New List(Of Synapse)
	Public Property Fitness As New List(Of Double)
	Public Sub New()
		LayerQTY = Settings.LayerQTY
		NeuronQTY = Settings.NeuronQTY
	End Sub
	Private ReadOnly Property totalNerons As Integer
		Get
			Return (LayerQTY * NeuronQTY) + Outputs
		End Get
	End Property
	Private ReadOnly Property totalSynapses As Integer
		Get
			Return CInt((Inputs * NeuronQTY) + (NeuronQTY ^ (LayerQTY)) + (Outputs * NeuronQTY))
		End Get
	End Property
	Private ReadOnly Property Layers As List(Of Layer)
		Get
			Dim List As New List(Of Layer)
			Dim NeuronIndex As Integer = 0

			'Populate hidden layers
			Dim Layer As New Layer
			For i = 1 To LayerQTY
				Layer = New Layer
				For j = 1 To NeuronQTY
					Layer.Neurons.Add(Neurons.Item(NeuronIndex))
					NeuronIndex += 1
				Next
				List.Add(Layer)
			Next

			'Populate output layer
			Layer = New Layer
			For j = 1 To Outputs
				Layer.Neurons.Add(Neurons.Item(NeuronIndex))
				NeuronIndex += 1
			Next
			List.Add(Layer)

			Return List
		End Get
	End Property
	Public ReadOnly Property AverageFitness As Double
		Get
			If Fitness.Count = 0 Then Return 0
			Dim Total As Double = 0
			For Each Score In Fitness
				Total += Score
			Next
			Return Total / Fitness.Count
		End Get
	End Property

	Public Sub Initialize()
		For i = 1 To totalNerons
			Neurons.Add(New Neuron With {.Bias = RNG(Settings.RNGBounds * -1, Settings.RNGBounds)})
		Next
		For i = 1 To totalSynapses
			Synapses.Add(New Synapse With {.Weight = RNG(Settings.RNGBounds * -1, Settings.RNGBounds)})
		Next
	End Sub

	Public Function Crossover(Spouse As Network) As List(Of Network)
		Dim Child1 As New Network()
		Dim Child2 As New Network()
		For Each Neuron In Neurons
			If RNGInt(0, 1) = 0 Then
				Child1.Neurons.Add(Neuron)
				Child2.Neurons.Add(Spouse.Neurons.Item(Neurons.IndexOf(Neuron)))
			Else
				Child1.Neurons.Add(Spouse.Neurons.Item(Neurons.IndexOf(Neuron)))
				Child2.Neurons.Add(Neuron)
			End If
		Next
		For Each Synapse In Synapses
			If RNGInt(0, 1) = 0 Then
				Child1.Synapses.Add(Synapse)
				Child2.Synapses.Add(Spouse.Synapses.Item(Synapses.IndexOf(Synapse)))
			Else
				Child1.Synapses.Add(Spouse.Synapses.Item(Synapses.IndexOf(Synapse)))
				Child2.Synapses.Add(Synapse)
			End If
		Next
		Dim List As New List(Of Network) From {Child1, Child2}
		Return List
	End Function
	Public Sub Mutate()
		For Each Neuron In Neurons
			If RNG(0, 1) < Settings.GeneMutatePercent Then
				Neuron.Bias = RNG(Settings.RNGBounds * -1, Settings.RNGBounds)
			End If
		Next
		For Each Synapse In Synapses
			If RNG(0, 1) < Settings.GeneMutatePercent Then
				Synapse.Weight = RNG(Settings.RNGBounds * -1, Settings.RNGBounds)
			End If
		Next
	End Sub
	Public Function Calculate(InputValues As List(Of Integer)) As Integer
		'For i = 0 To Inputs - 1
		'	Neurons.Item(i).Value = InputValues.Item(i)
		'Next
		Dim SynapseIndex As Integer = 0
		For i = 0 To Layers.Count - 1 'For each layer
			For j = 0 To Layers.Item(i).Neurons.Count - 1 'For each neuron in the layer
				Dim Neuron As Neuron = Layers.Item(i).Neurons.Item(j)
				If i = 0 Then
					For k = 0 To Inputs - 1 'For each synapse connected to inputs
						Neuron.Value += InputValues(k) * Synapses.Item(SynapseIndex).Weight
						SynapseIndex += 1
					Next
				Else
					For k = 0 To Layers.Item(i - 1).Neurons.Count - 1 'For each synapse connected to the neuron
						Neuron.Value += Neurons.Item(k).Value * Synapses.Item(SynapseIndex).Weight
						SynapseIndex += 1
					Next
				End If
				Neuron.Value += Neuron.Bias
				Neuron.Value = Sigmoid(Neuron.Value)
			Next
		Next
		Dim Result As Integer = GetMostActivated(Layers.Item(Layers.Count - 1).Neurons)
		ClearValues()
		Return Result
	End Function
	Private Function Sigmoid(x As Double) As Double
		Return 1 / (1 + Math.Exp(-x))
	End Function
	Private Function GetMostActivated(List As List(Of Neuron)) As Integer
		Dim MostActivated As Integer = 0
		For i = 1 To List.Count - 1
			If List.Item(i).Value > List.Item(MostActivated).Value Then
				MostActivated = i
			End If
		Next
		Return MostActivated + 1
	End Function
	Private Function ClearValues() As Boolean
		For Each Neuron In Neurons
			Neuron.Value = 0
		Next
		Return True
	End Function
End Class
Public Class Neuron
	Public Property Bias As Double
	Public Property Value As Double
End Class
Public Class Synapse
	Public Property Weight As Double
End Class
Public Class Layer
	Public Property Neurons As New List(Of Neuron)
End Class