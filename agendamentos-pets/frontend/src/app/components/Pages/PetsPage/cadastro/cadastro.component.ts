import { Component } from '@angular/core';
import { PetsService } from '../../../../services/Pet/pets.service';
import { Pet } from '../../../../Models/pet';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgForm } from '@angular/forms';


@Component({
  selector: 'app-cadastro',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cadastro.component.html',
  styleUrl: './cadastro.component.css'
})

export class CadastroComponent {
  mensagem: string = '';      // Para mensagens de erro/sucesso
  sucesso: boolean = false;   // Para definir se é sucesso ou erro
  exibirMensagem = false;
  racas: any[] = [];
  racasFiltradas: any[] = [];

  pet: Pet ={
    id: 0,
    nome: '',
    idade: 0,
    cor: '',
    sexo: '',
    tipo: '',
    nomeRaca: '',
    tutor: '',
    emailTutor: '',
    imagemUrl: ''
  }

  constructor(private petsService: PetsService) {}
  
  buscarRacas(especie: string) {
    this.petsService.buscarRacas(especie).subscribe((racas:any[]) => {
      // console.log("Dados recebidos: ", racas);
      this.racas = racas;
    });
  }

  cadastrarPet(form: NgForm) {
    if(this.pet.tutor === '')
    {
      this.mensagem = '❌ Tutor é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.nome === '')
    {
      this.mensagem = '❌ Nome é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.tipo === '')
    {
      this.mensagem = '❌ Tipo é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.nomeRaca === '') {
      this.mensagem = '❌ Raça é obrigatória!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.idade === 0) {
      this.mensagem = '❌ Idade é obrigatória!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.sexo === '') {
      this.mensagem = '❌ Sexo é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.cor === '') {
      this.mensagem = '❌ Cor é obrigatória!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }
    if(this.pet.emailTutor === '') {
      this.mensagem = '❌ Email do tutor é obrigatório!';
      this.sucesso = false;
      this.exibirMensagem = true;
      return;
    }

    this.petsService.addPet(this.pet).subscribe({
      next: (response) => {
        this.mensagem = '✅ Pet cadastrado com sucesso!';
        this.sucesso = true;  // Indica sucesso
        this.exibirMensagem = true;

        // Limpa o formulário
        form.resetForm();
      },

      error: (error) => {
        if (error.status === 400) {
          this.mensagem = '❌ Erro ao cadastrar pet. Verifique os dados.';
          this.sucesso = false; // Indica erro
          this.exibirMensagem = true;
        }
        else {
        console.error('Erro:', error);
        alert('Erro ao cadastrar pet.');
        }
      }
    });
  }

  navigateToPets() {
    window.location.href = '/pets';
  }

}
