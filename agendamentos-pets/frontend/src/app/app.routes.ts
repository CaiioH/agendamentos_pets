import { Component } from '@angular/core';
import { Routes } from '@angular/router';
import { HomeComponent } from './components/Pages/HomePage/home/home.component';
import { PetsComponent } from './components/Pages/PetsPage/pets/pets.component';
import { CadastroComponent } from './components/Pages/PetsPage/cadastro/cadastro.component';
import { EditarpetComponent } from './components/Pages/PetsPage/editarpet/editarpet.component';
import { AgendamentosComponent } from './components/Pages/AgendamentosPages/agendamentos/agendamentos.component';
import { NovoAgendamentoComponent } from './components/Pages/AgendamentosPages/novo-agendamento/novo-agendamento.component';
import { EditarAgendamentoComponent } from './components/Pages/AgendamentosPages/editar-agendamento/editar-agendamento.component';

export const routes: Routes = [
    {
        path: "",
        component: HomeComponent
    },
    {
        path: "home",
        component: HomeComponent
    },
    {
        path: "pets",
        component: PetsComponent
    },
    {
        path: "cadastro",
        component: CadastroComponent
    },
    {
        path: "editarpet/:id",
        component: EditarpetComponent
    },
    {
        path: "agendamentos",
        component: AgendamentosComponent
    },
    {
        path: "novo-agendamento",
        component: NovoAgendamentoComponent
    },
    {
        path: "editar-agendamento/:id",
        component: EditarAgendamentoComponent
    },
];
