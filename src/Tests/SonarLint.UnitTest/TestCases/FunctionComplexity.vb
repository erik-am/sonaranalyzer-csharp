Namespace Tests.Diagnostics
    Public Class FunctionComplexityVisualBasic
        Public Sub S1() ' Compliant
            If False

            End If
            If False

            End If
        End Sub

        Public Sub S2()  ' Noncompliant
            If False

            End If
            If False

            End If
            If False

            End If
        End Sub

        Public Function F1() As Integer  ' Compliant
            If False

            End If
            If False

            End If
            Return 0
        End Function

        Public Function F2() As Integer  ' Noncompliant
            If False

            End If
            If False

            End If
            If False

            End If
            Return 0
        End Function

        Public Property P1 As Integer
            Get ' Compliant
                If False

                End If
                If False

                End If
                Return 0
            End Get
            Set(value As Integer) ' Compliant
                If False

                End If
                If False

                End If
            End Set
        End Property

        Public Property P2 As Integer
            Get ' Noncompliant
                If False

                End If
                If False

                End If
                If False

                End If
                If False

                End If
                Return 0
            End Get
            Set(value As Integer) ' Noncompliant
                If False

                End If
                If False

                End If
                If False

                End If
                If False

                End If
            End Set
        End Property
    End Class
End Namespace