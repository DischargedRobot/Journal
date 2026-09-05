import { create } from "zustand"
import {
	FormValues,
	LOGIN_FIELDS,
	PERSONAL_FIELDS,
	REQUIRED_FIELDS,
} from "../fields"
import { error } from "console"

type TFormField = {
	value: string
	error: string
	required: boolean
}

type TFormValues = {
	[name in keyof FormValues]: TFormField
}

interface IFormStore {
	formValues: TFormValues
	updateField: (name: keyof FormValues, value: string) => void
	isValidatedField: <K extends keyof FormValues>(
		fieldName: K,
		fieldValue: FormValues[K],
	) => boolean
}

const useRegistrationFormStore = create<IFormStore>((set, get) => {
	const isRequired = (name: keyof FormValues): boolean => {
		return REQUIRED_FIELDS.includes(name)
	}

	const initialFields: TFormValues = [
		...PERSONAL_FIELDS,
		...LOGIN_FIELDS,
	].reduce((acc, name) => {
		acc[name] = {
			value: "",
			error: "",
			required: isRequired(name),
		}
		return acc
	}, {} as TFormValues)

	return {
		formValues: initialFields,
		updateField: (fieldName, fieldValue) => {
			set((state) => {
				return {
					formValues: {
						...state.formValues,
						[fieldName]: {
							...state.formValues[fieldName],
							value: fieldValue,
						},
					},
				}
			})
		},

		isValidatedField(fieldName, fieldValue) {
			const field = get().formValues[fieldName]
			if (!field) {
				return false
			}

			const isEmpty = fieldValue == null || fieldValue.trim() === ""
			// TODO: Сделать список с русскими именами полей и подставлять автоматом
			if (isEmpty) {
				if (field.required) {
					field.error = `Поле ${fieldName} обязательно для заполенения`
				}
				return !field.required
			}

			switch (fieldName) {
				case "email":
					field.error = "Неверный email"
					return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(fieldValue)

				case "firstName":
				case "lastName":
					field.error = `${fieldName} должно содержать только русские буквы, пробелы и тире`
					return /^[а-яА-ЯёЁ -]+$/.test(fieldValue)

				default:
					return true
			}
		},
	}
})

export default useRegistrationFormStore
