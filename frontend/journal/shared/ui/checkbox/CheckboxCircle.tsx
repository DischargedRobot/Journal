"use client"

import MuiCheckbox, { CheckboxProps as MuiCheckboxProps } from "@mui/material/Checkbox"
import CheckboxCircleCheckedIcon from "./CheckboxCircleCheckedIcon"
import CheckboxCircleUncheckedIcon from "./CheckboxCircleUncheckedIcon"

export type CheckboxCircleProps = MuiCheckboxProps

const iconSx = {
	padding: 0,
	width: 32,
	height: 32,
} as const

const CheckboxCircle = ({ sx, ...props }: CheckboxCircleProps) => (
	<MuiCheckbox
		{...props}
		icon={<CheckboxCircleUncheckedIcon />}
		checkedIcon={<CheckboxCircleCheckedIcon />}
		sx={[
			iconSx,
			...(Array.isArray(sx) ? sx : [sx]),
		]}
	/>
)

export default CheckboxCircle
