import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AgendaService } from '../../../../services/Agendamentos/agenda.service';
import { ConfirmDialogComponent } from '../../../confirm-dialog/confirm-dialog.component';
import { FormsModule } from '@angular/forms'; // ✅ Importação necessária!


// Importando módulos do Angular Material
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-editar-agendamento',
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatDatepickerModule,
    MatNativeDateModule,
    FormsModule 
  ],
  templateUrl: './editar-agendamento.component.html',
  styleUrl: './editar-agendamento.component.css'
})
export class EditarAgendamentoComponent implements OnInit {
  agendamentoSelecionado: any = {};
  agendamentoForm!: FormGroup;
  agendamentoId!: string;
  minDate: Date = new Date(); // Data mínima para agendamento sempre atualizada


  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private fb = inject(FormBuilder);

  constructor(
    private agendaService: AgendaService,
    private dialog: MatDialog
  ) {
    this.minDate.setDate(this.minDate.getDate() + 1); // Atualiza a data mínima para o dia seguinte
   }

  ngOnInit(): void {
    // Pegando o ID da URL
    this.agendamentoId = this.route.snapshot.paramMap.get('id') || '';

    // 🔥 Inicializa o formulário com valores vazios antes de carregar os dados do pet
    this.agendamentoForm = this.fb.group({
      servico: [''],
      descricao: [''],
      dataAgendamento: ['']
    });

    // Buscando os dados do agendamento para preencher o formulário
    this.agendaService.getAgendamentoById(this.agendamentoId).subscribe((agendamento) => {
      if (agendamento) {
        this.agendamentoSelecionado = agendamento;
        this.agendamentoForm.patchValue(agendamento);
      }
    });
  }

  atualizarAgendamento(): void {
    if (this.agendamentoForm.invalid) {
      this.agendamentoForm.markAllAsTouched(); // Isso destaca os erros nos campos
      return
    }
    else {
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        data: {
          title: 'Atualizar Agendamento',
          message: 'Tem certeza que deseja atualizar este agendamento?',
          confirmText: 'Sim',
          cancelText: 'Não',
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          const agendamentoAtualizado = {
            ...this.agendamentoSelecionado,
            ...this.agendamentoForm.value
          };

          this.agendaService.updateAgendamento(this.agendamentoId, agendamentoAtualizado).subscribe({
            next: () => {
              console.log('Agendamento atualizado com sucesso');
              this.router.navigate(['/agendamentos']);
            },
            error: (error) => {
              console.error('Ocorreu um erro ao atualizar o agendamento: ', error);
            }
          });
        }
      });
    }
  }

  fecharFormulario(): void {
    this.router.navigate(['/agendamentos']); // Voltar para a lista sem salvar
  }

}
