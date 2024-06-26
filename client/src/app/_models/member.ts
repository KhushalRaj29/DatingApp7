import { Photo } from "./photo";

export interface Member{
    id:number;
    userName:string;
    photoUrl:string;
    age:number;
    KnownAs:string;
    created:Date;
    lastActive:Date;
    gender:string;
    itroduction:string;
    lookingFor:string;
    interests:string;
    city:string;
    country:string;
    photos:Photo[];
}