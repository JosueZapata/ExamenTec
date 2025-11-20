import { StoreResponseDto } from './storeResponseDto';


export interface StoreResponseDtoPagedResult { 
    items?: Array<StoreResponseDto> | null;
    page?: number;
    pageSize?: number;
    totalCount?: number;
    readonly totalPages?: number;
    readonly hasPreviousPage?: boolean;
    readonly hasNextPage?: boolean;
}


