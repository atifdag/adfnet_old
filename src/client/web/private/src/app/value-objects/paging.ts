import { IdCodeName } from './id-code-name';

export class Paging {
    pageCount: number;
    totalItemCount: number;
    pageNumber: number;
    pageSize: number;
    hasPreviousPage: boolean;
    hasNextPage: boolean;
    isFirstPage: boolean;
    isLastPage: boolean;
    pageSizes: IdCodeName[];
}
