import { Component } from '@angular/core';
import { AgendaService } from '../../../../services/Agendamentos/agenda.service';
import { PetsService } from '../../../../services/Pet/pets.service';
import { Agenda } from '../../../../Models/Agenda';
import { Router } from '@angular/router'; // ✅ Usar Router para navegação
import { FormsModule } from '@angular/forms'; // Permite utilizar o (ngModel) no html
import { CommonModule } from '@angular/common'; // ✅ Importar CommonModule pra uso de date no html
import { NgForm } from '@angular/forms';


@Component({
  selector: 'app-novo-agendamento',
  imports: [FormsModule, CommonModule],
  templateUrl: './novo-agendamento.component.html',
  styleUrl: './novo-agendamento.component.css'
})
export class NovoAgendamentoComponent {
  minDate: Date = new Date(); // Data mínima para agendamento sempre atualizada
  pets: any[] = [];
  mensagem: string = '';      // Para mensagens de erro/sucesso
  sucesso: boolean = false;   // Para definir se é sucesso ou erro
  exibirMensagem = false;

  agendamento: Agenda = {
    id: 0,
    servico: '',
    descricao: '',
    petId: 0,
    nomePet: '',
    tutor: '',
    emailTutor: '',
    dataCriacao: '',
    dataAgendamento: ''
  }

  constructor(
    private agendaService: AgendaService,
    private petsService: PetsService, 
    private router: Router // ✅ Substitui `window.location.href`

  ) {
    this.minDate.setDate(this.minDate.getDate() + 1); // Atualiza a data mínima para o dia seguinte
    }

  listPets(): void {
    this.petsService.getPets().subscribe((pets: any[]) => {
      this.pets = pets.map(pet => ({ ...pet, id: String(pet.id) })); // Garante que o ID é string
    });
  }

  atualizarEmailTutor() {
    const petSelecionado = this.pets.find(pet => pet.nome === this.agendamento.nomePet);
    if (petSelecionado) {
      this.agendamento.emailTutor = petSelecionado.emailTutor;
    }
  }

  novoAgendamento(form: NgForm) {
    if(this.agendamento.servico === '') {
      this.mensagem = '❌ Serviço é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.agendamento.nomePet === '') {
      this.mensagem = '❌ Pet é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.agendamento.dataAgendamento === '') {
      this.mensagem = '❌ Data é obrigatória!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }

    this.agendaService.novoAgendamento(this.agendamento).subscribe({
      next: (response) => {
        this.mensagem = '✅ Agendado com sucesso!';
        this.sucesso = true;  // Indica sucesso
        this.exibirMensagem = true;
        
        // Limpa o formulário
        form.resetForm();

      },
      error: (error) => {
        console.error('Erro:', error);

      }
    });
  }

  navigateToAgendamentos() {
    this.router.navigate(['/agendamentos']); // ✅ Uso do Router

  }

}
