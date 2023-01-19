Option Explicit On
Option Strict On
Imports System.Collections.Generic
Public Class Population
	Public Property Networks As New List(Of Network)
	Private Settings As Settings
	Public Sub New(Setting As Settings)
		Settings = Setting
		For i = 1 To Settings.PopulationSize
			Networks.Add(New Network)
		Next

		SavePopulation(Me, 1)
	End Sub
End Class
Public Class Network
	Private ReadOnly Property Inputs As Integer = 6
	Private ReadOnly Property Outputs As Integer = 4
	Private ReadOnly Property LayerQTY As Integer = 2
	Private ReadOnly Property NeuronQTY As Integer = 6
	Public Property Neurons As New List(Of Neuron)
	Public Property Synapses As New List(Of Synapse)
	Public Property Fitness As New List(Of Double)
	Public Sub New()
		For i = 1 To totalNerons
			If i <= Inputs Then
				Neurons.Add(New Neuron)
			Else
				Neurons.Add(New Neuron With {.Bias = RNG(-10, 10)})
			End If
		Next
		For i = 1 To totalSynapses
			Synapses.Add(New Synapse With {.Weight = RNG(-5, 5)})
		Next
	End Sub
	Private ReadOnly Property totalNerons As Integer
		Get
			Return Inputs + (LayerQTY * NeuronQTY) + Outputs
		End Get
	End Property
	Private ReadOnly Property totalSynapses As Integer
		Get
			Return CInt((Inputs * NeuronQTY) + (NeuronQTY ^ (LayerQTY - 1)) + (Outputs * NeuronQTY))
		End Get
	End Property
	Private ReadOnly Property Layers As List(Of Layer)
		Get
			Dim List As New List(Of Layer)
			Dim NeuronIndex As Integer = 0

			'Populate input layer
			Dim Layer As New Layer
			For j = 1 To Inputs
				Layer.Neurons.Add(Neurons.Item(NeuronIndex))
				NeuronIndex += 1
			Next
			List.Add(Layer)

			'Populate hidden layers
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

	Public Function Calculate(InputValues As List(Of Integer)) As Integer
		For i = 0 To Inputs - 1
			Neurons.Item(i).Value = InputValues.Item(i)
		Next
		'For Each Neuron In Layers.Item(0).Neurons
		'	Neuron.Value = Sigmoid(Neuron.Value)
		'Next
		For i = 1 To Layers.Count - 1 'For each layer after the input layer
			For j = 0 To Layers.Item(i).Neurons.Count - 1 'For each neuron in the layer
				Dim Neuron As Neuron = Layers.Item(i).Neurons.Item(j)
				Dim SynapseIndex As Integer = 0
				For k = 0 To Layers.Item(i - 1).Neurons.Count - 1 'For each synapse connected to the neuron
					Neuron.Value += Neurons.Item(k).Value * Synapses.Item(SynapseIndex).Weight
					SynapseIndex += 1
				Next
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