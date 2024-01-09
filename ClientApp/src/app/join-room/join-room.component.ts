import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ChatService } from '../chat.service';

@Component({
  selector: 'app-join-room',
  templateUrl: './join-room.component.html',
  styleUrls: ['./join-room.component.css']
})
export class JoinRoomComponent implements OnInit {

  joinRoomForm!: FormGroup;
  fb = inject(FormBuilder);
  router = inject(Router);
  chatService = inject(ChatService);

  ngOnInit(): void {
    this.joinRoomForm = this.fb.group({
      user: ['', Validators.required],
      room: ['', Validators.required],
      email: ['', Validators.required]
    });
  }

  joinRoom(){
    const {user, room, email} = this.joinRoomForm.value;
    sessionStorage.setItem("user", user);
    sessionStorage.setItem("room", room);
    sessionStorage.setItem("email", email);
    this.chatService.joinRoom(user, room, email)
    .then(()=>{
      this.router.navigate(['chat']);
    }).catch((err)=>{
      console.log(err);
    })

  }

}
