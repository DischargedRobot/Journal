import { TFaculty } from "../t-faculty/TFaculty"
import { IBaseEntityWithVersion } from "../utility-types/base-entity"
import { Uuid } from "../utility-types/uuid"
import { TProfessor } from "../professor/TProfessor"
import { TTrainingDirection } from "../training-direction"

export type TGroup = {
	admissionDate: string // DateOnly сериализуется как строка (YYYY-MM-DD)
	code: string
	trainingDirection: TTrainingDirection
	faculty: TFaculty
	curators: TProfessor[]
} & IBaseEntityWithVersion
