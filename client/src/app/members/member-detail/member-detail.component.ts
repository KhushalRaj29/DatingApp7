import { Component, OnInit } from '@angular/core';
import { Member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css',
  imports:[CommonModule,TabsModule,GalleryModule, TimeagoModule]
})
export class MemberDetailComponent implements OnInit {
member:Member | undefined;
images:GalleryItem[]=[];

  constructor(private memberService:MembersService, private route:ActivatedRoute) {}

  ngOnInit(): void {
    this.loadMember(); 
  }

  loadMember(){
    const username=this.route.snapshot.paramMap.get('username');
    if(!username) return;
    this.memberService.getMember(username).subscribe({
      next:member=>{
        this.member=member,
        this.getImages()
      }
    })
  }

  getImages(){
    if(!this.member) return;
    for(const photo of this.member?.photos)
      {
          this.images.push(new ImageItem({src:photo.url, thumb:photo.url}))
      }
  }
  
}
