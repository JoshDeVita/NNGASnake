Imports System.Collections.Generic
Public Class Population
	Private ReadOnly Property Networks As New List(Of Network)
	Private Settings As Settings
	Public Sub New(Setting As Settings)
		Settings = Setting
		Dim swLog As New IO.StreamWriter(Settings.FolderPath & "Generation 1.csv", False)
		swLog.Close()

		For i = 1 To Settings.PopulationSize
			Networks.Add(New Network)
			SaveNetwork(Settings.FolderPath & "Generation 1.csv", Networks.Item(Networks.Count - 1))
		Next
	End Sub
End Class
Public Class Network
	Private ReadOnly Property Inputs As Integer = 4
	Private ReadOnly Property Outputs As Integer = 4
	Private ReadOnly Property LayerQTY As Integer = 2
	Private ReadOnly Property NeuronQTY As Integer = 6
	Public Property Neurons As New List(Of Neuron)
	Public Property Synapses As New List(Of Synapse)
	Public Sub New()
		For i = 1 To totalNerons
			Neurons.Add(New Neuron With {.Bias = RNG(-5, 5)})
		Next
		For i = 1 To totalSynapses
			Synapses.Add(New Synapse With {.Weight = RNG(-2, 2)})
		Next
	End Sub
	Private ReadOnly Property totalNerons As Integer
		Get
			Return Inputs + (LayerQTY * NeuronQTY) + Outputs
		End Get
	End Property
	Private ReadOnly Property totalSynapses As Integer
		Get
			Return (Inputs * NeuronQTY) + (NeuronQTY ^ (LayerQTY - 1)) + (Outputs * NeuronQTY)
		End Get
	End Property
End Class
Public Class Neuron
	Public Property Bias As Double
	Public Property Value As Double
End Class
Public Class Synapse
	Public Property Weight As Double
End Class