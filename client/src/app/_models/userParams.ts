import { User } from "./user";

export class userParams{
    gender:string;
    minAge=18;
    maxAge=99;
    pageNumber=1;
    pageSize=3;
    orderby='lastActive';

    constructor(user:User){
        this.gender=user.gender === 'female' ? 'male' : 'female'
    }

}
