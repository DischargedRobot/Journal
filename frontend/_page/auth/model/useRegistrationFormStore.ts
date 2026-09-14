import { create } from "zustand"
import { FormValues, REQUIRED_FIELDS } from "../fields"

type TFormField<T> = {
	value: T
	error: string
	required: boolean
}

export type TFormValues = {
	[name in keyof Required<FormValues>]: TFormField<FormValues[name]>
}

interface IFormStore {
	formValues: TFormValues
	updateField: <K extends keyof FormValues>(
		name: K,
		value?: FormValues[K],
		error?: string,
	) => void
	isValidatedField: <K extends keyof FormValues>(
		fieldName: K,
		fieldValue: FormValues[K],
	) => { error: string; isValidated: boolean }
}

const useRegistrationFormStore = create<IFormStore>((set, get) => {
	const isRequired = (name: keyof FormValues): boolean => {
		return REQUIRED_FIELDS.includes(name)
	}

	const createField = <K extends keyof FormValues>(
		name: K,
		value: FormValues[K],
	): TFormField<FormValues[K]> => ({
		value,
		error: "",
		required: isRequired(name),
	})

	const initialFields: TFormValues = {
		firstName: createField("firstName", ""),
		lastName: createField("lastName", ""),
		patronymic: createField("patronymic", ""),
		email: createField("email", ""),
		personRole: createField("personRole", "СТУДЕНТ"),
		group: createField("group", null),
		department: createField("department", null),
		login: createField("login", ""),
		password: createField("password", ""),
		passwordConfirm: createField("passwordConfirm", ""),
	}

	return {
		formValues: initialFields,
		updateField: (fieldName, fieldValue, fieldError) => {
			console.log("updateField START")
			let error = ""
			if (fieldError) {
				error = fieldError
			} else if (fieldValue) {
				error = get().isValidatedField(fieldName, fieldValue).error
			}

			set((state) => ({
				formValues: {
					...state.formValues,
					[fieldName]: {
						...state.formValues[fieldName],
						value: fieldValue,
						error,
					},
				},
			}))
			console.log("updateField END")
		},

		isValidatedField(fieldName, fieldValue) {
			const field = get().formValues[fieldName]

			if (!field) {
				return {
					error: "Поле отсутствует",
					isValidated: false,
				}
			}

			const isEmpty = fieldValue == null || fieldValue.trim() === ""

			if (isEmpty) {
				return field.required
					? {
							error: `Поле ${fieldName} обязательно для заполнения`,
							isValidated: false,
						}
					: {
							error: "",
							isValidated: true,
						}
			}

			switch (fieldName) {
				case "email": {
					const isValidated = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(
						fieldValue,
					)

					return {
						error: isValidated ? "" : "Неверный email",
						isValidated,
					}
				}

				case "firstName":
				case "lastName": {
					const isValidated = /^[а-яА-ЯёЁ -]+$/.test(fieldValue)

					return {
						error: isValidated
							? ""
							: `${fieldName} должно содержать только русские буквы, пробелы и тире`,
						isValidated,
					}
				}

				case "password":
					const isValidated = !/\s/.test(fieldValue)
					return {
						error: isValidated
							? ""
							: "Пароль не может содержать пробелы",
						isValidated,
					}
				case "passwordConfirm": {
					const password = get().formValues.password.value.trim()
					const isValidated = fieldValue.trim() === password
					return {
						error: isValidated ? "" : `Пароли не совпадают`,
						isValidated,
					}
				}

				default:
					return {
						error: "",
						isValidated: true,
					}
			}
		},
	}
})

export default useRegistrationFormStore
