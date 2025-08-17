import { Component } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../confirm-dialog/confirm-dialog.component';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router'; // ✅ Usar Router para navegação
import { AgendaService } from '../../../../services/Agendamentos/agenda.service';
import { CommonModule } from '@angular/common';
import { Agenda } from '../../../../Models/Agenda';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-agenda',
  standalone: true,
  templateUrl: './agendamentos.component.html',
  styleUrls: ['./agendamentos.component.css'],
  imports: [MatDialogModule, FormsModule, CommonModule, NgxPaginationModule, MatIcon]
})
export class AgendamentosComponent {
  paginaAtual: number = 1;  // Página atual
  itensPorPagina: number = 10;  // Quantos pets por página
  agendamentos: Agenda[] = [];
  agendamentosFiltrados: Agenda[] = [];
  filtroSelecionado: string = '';
  valorFiltro: string = '';
  expandedAgendaId: number | null = null; // ID do agendamento expandido

  constructor(
    private agendaService: AgendaService,
    private dialog: MatDialog,
    private router: Router // ✅ Substitui `window.location.href`
  ) { }

  ngOnInit(): void {
    this.listAgendamentos();
  }

  listAgendamentos(): void {
    this.agendaService.getAgendamentos().subscribe((agendamentos: Agenda[]) => {
      this.agendamentos = agendamentos;
      this.agendamentosFiltrados = [...this.agendamentos]; // Inicia sem filtros
    });
  }

  navigateToCadastro() {
    this.router.navigate(['/novo-agendamento']); // ✅ Uso do Router
  }

  navigateToEdit(agendaId: number) {
    this.router.navigate(['/editar-agendamento', agendaId]); // ✅ Melhor navegação
  }

  expandeDetalhe(agendaId: number): void {
    this.expandedAgendaId = this.expandedAgendaId === agendaId ? null : agendaId;
  }

  deleteAgendamento(agendaId: any) {
    const confirmar = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Cancelar Agendamento',
        message: 'Tem certeza que deseja cancelar este agendamento?',
        confirmText: 'Confirmar',
        cancelText: 'Cancelar',
        buttonColor: 'warn'
      }
    });

    confirmar.afterClosed().subscribe(result => {
      if (result) {
        this.agendaService.deleteAgendamento(agendaId).subscribe({
          next: () => {
            this.agendamentos = this.agendamentos.filter(a => a.id !== agendaId);

            //Essa validação impede erros caso this.filtroSelecionado tenha um valor inválido.
            if (!this.filtroSelecionado || !(this.filtroSelecionado in this.agendamentos[0])) {
              this.agendamentosFiltrados = [...this.agendamentos];
              return;
            }

            //Isso informa ao TypeScript que key é uma chave válida da interface Agenda, evitando o erro.
            const key = this.filtroSelecionado as keyof Agenda;

            this.agendamentosFiltrados = this.agendamentos.filter(agendamento =>
              !this.filtroSelecionado ||
              agendamento[key]?.toString().toLowerCase().includes(this.valorFiltro.toLowerCase())
            );
          },
          error: (error) => {
            console.error('Erro ao cancelar agendamento:', error);
          }
        });
      }
    });
  }

  filtrarAgendamentos(): void {
    if (!this.filtroSelecionado || !this.valorFiltro.trim()) {
      this.agendamentosFiltrados = [...this.agendamentos];
      return;
    }
    const key = this.filtroSelecionado as keyof Agenda;

    this.agendamentosFiltrados = this.agendamentos.filter(agendamento =>
      !this.filtroSelecionado ||
      agendamento[key]?.toString().toLowerCase().includes(this.valorFiltro.toLowerCase())
    );
    this.paginaAtual = 1; // Reseta para a primeira página ao filtrar

  }

  limparFiltro(): void {
    this.filtroSelecionado = '';
    this.valorFiltro = '';
    this.agendamentosFiltrados = [...this.agendamentos];
  }
}
