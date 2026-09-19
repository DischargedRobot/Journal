import Autocomplete from "@mui/material/Autocomplete"
import { memo, ReactNode } from "react"
import useRegistrationFormStore, {
	TFormValues,
} from "../../model/useRegistrationFormStore"
import TextField from "@mui/material/TextField"

interface Props {
	items: { code: string }[]
	fieldName: keyof TFormValues
	label?: ReactNode
}

const RegistrationAutocomplete = (props: Props) => {
	const { items, fieldName, label } = props

	const selectedItem = useRegistrationFormStore(
		(state) => state.formValues[fieldName],
	)
	const updateField = useRegistrationFormStore((state) => state.updateField)
	return (
		<Autocomplete
			freeSolo
			options={items
				.map((i) => i.code)
				.filter(
					(i, index, self) =>
						index ===
						self.findIndex((findingItem) => findingItem == i),
				)}
			value={selectedItem.value}
			onChange={(_, newValue) => {
				// console.log(newValue, "newValue", fieldName)
				// Выбрали группу из списка
				updateField(fieldName, newValue ?? "")
			}}
			onInputChange={(_, newInputValue) => {
				// Пользователь вводит своё значение
				updateField(fieldName, newInputValue)
			}}
			disabled={items.length === 0}
			renderInput={(params) => (
				<TextField
					{...params}
					variant="outlined"
					required
					label={label}
					size="small"
					error={!!selectedItem.error}
					helperText={
						selectedItem.error.length > 0 ? selectedItem.error : " "
					}
				/>
			)}
		/>
	)
}

export default memo(RegistrationAutocomplete)
