import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { TPagedRequestOptions, TPagedResponse } from "@/shared/api/TPaged"
import { TSortOrder } from "@/shared/api/TSortOrder"

const DISCIPLINE_URL =
	process.env.NEXT_PUBLIC_API_DISCIPLINE_URL_V1 ??
	"http://localhost:8080/api/v1/disciplines"

export type TDisciplinesCreateDto = {
	name: string
	shortName?: string | null
	isArchived?: boolean
	disciplineRegisterUuid?: string | null
	semesterUuid: string
	academicYearUuid: string
	groupsUuids: string[]
	professorsUuids?: string[] | null
}

export type TDisciplinesUpdateDto = {
	name?: string | null
	shortName?: string | null
	isArchived?: boolean | null
	disciplineRegisterUuid?: string | null
	semesterUuid?: string | null
	academicYearUuid?: string | null
	groupsUuids?: string[] | null
	professorsUuids?: string[] | null
}

export type TDisciplinesResponseDto = {
	uuid: string
	name: string
	shortName: string
	isArchived: boolean
	disciplineRegisterUuid?: string | null
	semesterUuid: string
	academicYearUuid: string
	groupsUuids: string[]
	professorsUuids: string[]
	version: number
}

// билдер запроса
function buildQuery(
	params: Record<string, string | number | boolean | unknown>,
) {
	const qs = Object.entries(params)
		.filter(([, v]) => v !== undefined && v !== null && v !== "")
		.map(
			([k, v]) =>
				`${encodeURIComponent(k)}=${encodeURIComponent(String(v))}`,
		)
		.join("&")
	return qs.length ? `?${qs}` : ""
}

export const DisciplineApi = {
	getDisciplines: async (
		options?: TPagedRequestOptions & {
			name?: string
			isArchived?: boolean
		},
	): Promise<TPagedResponse<TDisciplinesResponseDto>> => {
		const q = buildQuery(options ?? {})
		return ApiJsonRequest(`${DISCIPLINE_URL}${q}`, {
			method: "GET",
		})
	},

	getDisciplineByUuid: async (uuid: string) => {
		return ApiJsonRequest(`${DISCIPLINE_URL}/${uuid}`, {
			method: "GET",
		})
	},

	getLessonsByDiscipline: async (
		uuid: string,
		options?: TPagedRequestOptions,
	): Promise<TPagedResponse<TDisciplinesResponseDto>> => {
		const q = buildQuery(options ?? {})
		return ApiJsonRequest(`${DISCIPLINE_URL}/${uuid}/lessons${q}`, {
			method: "GET",
		})
	},

	getDisciplinesByGroup: async (
		groupUuid: string,
		options?: TPagedRequestOptions & {
			name?: string
			isArchived?: boolean
		},
	): Promise<TPagedResponse<TDisciplinesResponseDto>> => {
		const q = buildQuery(options ?? {})
		return ApiJsonRequest(`${DISCIPLINE_URL}/group/${groupUuid}${q}`, {
			method: "GET",
		})
	},

	getDisciplinesByProfessor: async (
		professorUuid: string,
		options?: TPagedRequestOptions & {
			name?: string
			isArchived?: boolean
		},
	): Promise<TPagedResponse<TDisciplinesResponseDto>> => {
		const q = buildQuery(options ?? {})
		return ApiJsonRequest(
			`${DISCIPLINE_URL}/professor/${professorUuid}${q}`,
			{
				method: "GET",
			},
		)
	},

	createDiscipline: async (data: TDisciplinesCreateDto) => {
		return ApiJsonRequest(`${DISCIPLINE_URL}`, {
			method: "POST",
			body: JSON.stringify(data),
		})
	},

	updateDiscipline: async (uuid: string, data: TDisciplinesUpdateDto) => {
		return ApiJsonRequest(`${DISCIPLINE_URL}/${uuid}`, {
			method: "PATCH",
			body: JSON.stringify(data),
		})
	},

	deleteDiscipline: async (uuid: string) => {
		return ApiJsonRequest(`${DISCIPLINE_URL}/${uuid}`, {
			method: "DELETE",
		})
	},
}
