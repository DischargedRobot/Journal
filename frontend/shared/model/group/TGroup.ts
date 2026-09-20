import { TFaculty } from "../t-faculty/TFaculty"
import {
	BrandedUuid,
	IBaseEntityWithVersion,
} from "../utility-types/base-entity"
import { TProfessor } from "../professor/TProfessor"
import { TTrainingDirection } from "../training-direction"
import { Uuid } from "../utility-types/uuid"
import { TWithoutEnrich } from "@/shared/lib/enricher/enrichTypes"
declare const GroupBrand: unique symbol
export type GroupUuid = BrandedUuid<typeof GroupBrand>

export const createTGroupUuid = (uuid: Uuid): GroupUuid => uuid as GroupUuid

export type TGroup = {
	admissionDate: string // DateOnly сериализуется как строка (YYYY-MM-DD)
	code: string
	trainingDirection: TTrainingDirection
	faculty: TFaculty
	curators: TProfessor[]
} & IBaseEntityWithVersion<GroupUuid>

export type TGroupWE = TWithoutEnrich<TGroup>
