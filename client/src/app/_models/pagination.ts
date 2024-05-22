export interface Pagination{
    currentPage:number;
    itemsPerPage:number;
    totalItems:number;
    totalPages:number;
}

// export class PaginatedResult<T>{
//     reuslt?:T;
//     pagination?:Pagination;
    
// }
export class PaginatedResult<T>{
    result?:T;
    pagination?:Pagination;   
}