import TextField, { TextFieldProps } from "@mui/material/TextField"
import { HTMLInputTypeAttribute, memo, ReactNode, useState } from "react"
import useRegistrationFormStore, {
	TFormValues,
} from "../../model/useRegistrationFormStore"
import { PasswordStregth } from "@/shared/ui/PasswordStregth"
import { InputAdornment } from "@mui/material"
import { IconButton } from "@mui/material"
import { Visibility, VisibilityOff } from "@mui/icons-material"

interface Props {
	label: string
	size: "small" | "medium"
	type?: HTMLInputTypeAttribute
	errorMessage?: string
	helperText?: ReactNode
	required?: boolean
	fieldName: keyof TFormValues
	slotProps?: TextFieldProps["slotProps"]
}

const RegistrationTextField = (props: Props) => {
	const {
		label,
		size,
		type,
		errorMessage,
		helperText,
		required,
		fieldName,
		slotProps,
	} = props

	const field = useRegistrationFormStore(
		(state) => state.formValues[fieldName],
	)
	const updateField = useRegistrationFormStore((state) => state.updateField)

	let visibleHelperText: ReactNode =
		helperText ?? (field?.error.length > 0 ? field.error : " ")

	// console.log(
	// 	visibleHelperText,
	// 	"visibleHelperText",
	// 	helperText,
	// 	field?.error,
	// 	fieldName,
	// )

	return (
		<TextField
			variant="outlined"
			label={label}
			size={size}
			type={type}
			required={required ?? field?.required}
			error={errorMessage ? !!errorMessage : !!field?.error}
			helperText={visibleHelperText}
			value={field?.value ?? ""}
			onChange={(event) => updateField(fieldName, event.target.value)}
			slotProps={slotProps}
		/>
	)
}

export const PasswordRegistrationTextField = memo((props: Props) => {
	const { fieldName } = props
	const field = useRegistrationFormStore((state) => state.formValues.password)
	const [showPassword, setShowPassword] = useState(false)
	return (
		<RegistrationTextField
			{...props}
			helperText={
				fieldName === "password" ? (
					<PasswordStregth password={field?.value ?? ""} />
				) : (
					props.helperText
				)
			}
			type={showPassword ? "text" : "password"}
			slotProps={{
				input: {
					endAdornment: (
						<InputAdornment position="end">
							<IconButton
								onClick={() =>
									setShowPassword((state) => !state)
								}
								onMouseDown={(event) => event.preventDefault()}
							>
								{showPassword ? (
									<VisibilityOff />
								) : (
									<Visibility />
								)}
							</IconButton>
						</InputAdornment>
					),
				},
			}}
		/>
	)
})

export default memo(RegistrationTextField)
